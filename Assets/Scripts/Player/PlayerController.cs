using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
	[Header("Movement Settings")]
	[SerializeField] public float moveSpeed = 7f;
	[SerializeField] public float jumpForce = 8f;

	[Header("Roll Settings")]
	[SerializeField] private float rollSpeed = 8f;
	[SerializeField] private float rollDuration = 0.3f;
	[SerializeField] private int maxRolls = 2;
	[SerializeField] private float rollCooldown = 1.5f;

	private bool isRolling = false;
	private int rollCount = 0;
	private bool isOnCooldown = false;

	[Header("Components")]
	[SerializeField] private Rigidbody2D playerRigidbody;
	[SerializeField] private Animator playerAnimator;
	[SerializeField] private BoxCollider2D playerCollider;
	[SerializeField] private LayerMask terrainLayer;
	[SerializeField] private PlayerAttack playerAttack;
	[SerializeField] private SpellData[] availableSpells;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private int currentSpellIndex = 0;
	private bool canMove = true;
	private bool jumpUsed = false;
	private bool isKnockedBack = false;
	private float knockbackDuration = 0.3f;
	private float currentKnockbackTimer = 0f;
	private bool isAttacking = false;
	private Coroutine attackRoutine;

	// Spell Idle State
	private bool isHoldingSpellIdle = false;
	private Coroutine spellIdleCoroutine;
	private float spellIdleDuration = 20f;

	// Input values
	private float horizontalInput;
	private bool jumpPressed;
	private bool rollPressed;
	private bool attackPressed;
	private bool switchSpellPressed;

	private void Start()
	{
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAttack.currentSpell = availableSpells[currentSpellIndex];
		SetAnimatorIdleState(); // chuyển idle ban đầu
	}

	private void Update()
	{
		HandleKnockbackState();
		HandleInput();

		HandleSpellSwitch(); // chuyển phép bằng Q

		if (CanReceiveInput())
		{
			if (rollPressed && rollCount < maxRolls && !isOnCooldown && IsGrounded() && !isAttacking)
			{
				StartCoroutine(PerformRoll());
			}

			if (attackPressed)
			{
				TryAttack();
			}

			Move();
		}

		HandleJump();
		UpdateAnimator();

		attackPressed = false;
		switchSpellPressed = false;
	}

	private void HandleInput()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		jumpPressed = Input.GetKeyDown(KeyCode.Space);
		rollPressed = Input.GetKeyDown(KeyCode.LeftShift);
		attackPressed = Input.GetMouseButtonDown(0);
		switchSpellPressed = Input.GetKeyDown(KeyCode.Q);
	}

	private void HandleSpellSwitch()
	{
		if (switchSpellPressed)
		{
			int nextIndex = (currentSpellIndex + 1) % availableSpells.Length;
			SwitchSpell(nextIndex);
		}
	}

	private void SwitchSpell(int index)
	{
		if (index >= 0 && index < availableSpells.Length)
		{
			currentSpellIndex = index;
			playerAttack.currentSpell = availableSpells[currentSpellIndex];
			SetAnimatorIdleState();
			Debug.Log($"[Spell] Chuyển sang phép: {availableSpells[currentSpellIndex].name}");
		}
	}

	private void SetAnimatorIdleState()
	{
		if (playerAnimator != null && playerAttack.currentSpell != null)
		{
			int spellState = playerAttack.currentSpell.StateIntAnim;
			playerAnimator.SetInteger("State", spellState);
			Debug.Log($"[Idle] Chuyển sang idle phép: {spellState}");

			isHoldingSpellIdle = true;

			// Nếu đang đếm trước đó thì hủy
			if (spellIdleCoroutine != null) StopCoroutine(spellIdleCoroutine);
			spellIdleCoroutine = StartCoroutine(SpellIdleCountdown());
		}
	}

	private IEnumerator SpellIdleCountdown()
	{
		yield return new WaitForSeconds(spellIdleDuration);

		if (isHoldingSpellIdle)
		{
			playerAnimator.SetInteger("State", 0); // về idle thường
			Debug.Log("[Idle] Hết thời gian → về idle mặc định (0)");
			isHoldingSpellIdle = false;
		}
	}

	private void CancelSpellIdle()
	{
		if (isHoldingSpellIdle)
		{
			isHoldingSpellIdle = false;
			playerAnimator.SetInteger("State", 0);
			if (spellIdleCoroutine != null) StopCoroutine(spellIdleCoroutine);
			Debug.Log("[Idle] Ngắt idle phép do hành động");
		}
	}


	private bool CanReceiveInput()
	{
		return canMove && !isRolling && !isKnockedBack && !isAttacking;
	}

    private void Move()
    {
        if (isAttacking || !canMove) return;

        playerRigidbody.velocity = new Vector2(horizontalInput * moveSpeed, playerRigidbody.velocity.y);

        if (horizontalInput != 0)
        {
            CancelSpellIdle();

            // Flip sprite thay vì flip toàn player
            spriteRenderer.flipX = horizontalInput < 0;
        }
    }

    private void HandleJump()
	{
		if (jumpPressed)
		{
			//CancelSpellIdle();
			if (isRolling || isAttacking || isKnockedBack) return; // Không nhảy khi đang lăn, tấn công hay bị đẩy lùi

			if (IsGrounded())
			{
				CancelSpellIdle();
				Jump();
				jumpUsed = false;
			}
			else if (!jumpUsed)
			{
				CancelSpellIdle();
				Jump();
				jumpUsed = true;
			}
		}
	}

	private void Jump()
	{
		playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
	}

	private bool IsGrounded()
	{
		return Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f,
			Vector2.down, 0.1f, terrainLayer);
	}

	private void TryAttack()
	{
		CancelSpellIdle();
		if (isAttacking || isRolling || !IsGrounded()) return;
		if (!playerAttack.IsValidAttackAngle())
		{
			Debug.Log("[Attack] Chuột không nằm trong vùng bắn hợp lệ!");
			return;
		}

		if (attackRoutine != null) StopCoroutine(attackRoutine);
		attackRoutine = StartCoroutine(AttackRoutine());
	}

	private IEnumerator AttackRoutine()
	{
		isAttacking = true;
		canMove = false;

		playerRigidbody.velocity = Vector2.zero;

		if (playerAnimator != null && playerAttack.currentSpell != null)
		{
			string trigger = playerAttack.currentSpell.animationTrigger;
			playerAnimator.SetTrigger(trigger);
		}

		yield return new WaitForSeconds(0.8f);

		// Reset trigger tránh stuck
		if (playerAnimator != null && playerAttack.currentSpell != null)
		{
			playerAnimator.ResetTrigger(playerAttack.currentSpell.animationTrigger);
		}

		SetAnimatorIdleState(); // về idle tương ứng
		canMove = true;
		isAttacking = false;
	}

	public void PerformAttack() // animation event gọi hàm này
	{
		if (playerAttack != null)
		{
			playerAttack.PerformAttack();
		}
	}

	private void UpdateAnimator()
	{
		if (!canMove || isHoldingSpellIdle) return;

		playerAnimator.SetBool("IsMove", Mathf.Abs(playerRigidbody.velocity.x) > 0.1f);

		if (playerRigidbody.velocity.y > .1f)
		{
			playerAnimator.SetInteger("State", 1);
		}
		else if (playerRigidbody.velocity.y < -.1f)
		{
			playerAnimator.SetInteger("State", -1);
		}
		else
		{
			playerAnimator.SetInteger("State", 0);
		}
	}

	public void TriggerDeathAnimation()
	{
		canMove = false;
		isKnockedBack = false;
		playerAnimator.SetTrigger("IsDead");
		playerRigidbody.velocity = Vector2.zero;
		playerRigidbody.isKinematic = true;
	}

	public void ResetAfterRespawn()
	{
		canMove = true;
		isRolling = false;
		isKnockedBack = false;
		currentKnockbackTimer = 0f;
		rollCount = 0;
		isOnCooldown = false;

		playerRigidbody.isKinematic = false;
		playerRigidbody.velocity = Vector2.zero;

		playerAnimator.ResetTrigger("IsDead");
		playerAnimator.Play("Player_Idle");
	}

	private IEnumerator PerformRoll()
	{
		CancelSpellIdle();
		isRolling = true;
		canMove = false;
		rollCount++;

		float direction = transform.localScale.x > 0 ? 1f : -1f;
		playerRigidbody.velocity = new Vector2(direction * rollSpeed, 0f);

		playerAnimator.SetTrigger("IsRoll");

		yield return new WaitForSeconds(rollDuration);

		isRolling = false;
		canMove = true;

		if (rollCount >= maxRolls)
		{
			isOnCooldown = true;
			yield return new WaitForSeconds(rollCooldown);
			rollCount = 0;
			isOnCooldown = false;
		}
	}

	public void SetCanMove(bool value)
	{
		canMove = value;
	}

	void HandleKnockbackState()
	{
		if (isKnockedBack)
		{
			currentKnockbackTimer -= Time.deltaTime;
			if (currentKnockbackTimer <= 0)
			{
				isKnockedBack = false;
			}
		}
	}

	public void ApplyKnockback(Vector2 direction, float force)
	{
		if (!canMove) return;
		isKnockedBack = true;
		currentKnockbackTimer = knockbackDuration;
		playerRigidbody.velocity = Vector2.zero;
		playerRigidbody.AddForce(direction * force, ForceMode2D.Impulse);
	}
}
