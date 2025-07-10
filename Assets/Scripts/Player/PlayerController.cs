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

	private int currentSpellIndex = 0;
	private bool canMove = true;
	private bool jumpUsed = false;
	private bool isKnockedBack = false;
	private float knockbackDuration = 0.3f;
	private float currentKnockbackTimer = 0f;
	private bool isAttacking = false;
	private Coroutine attackRoutine;

	// Input values
	private float horizontalInput;
	private bool jumpPressed;
	private bool rollPressed;
	private bool attackPressed = false;

	private void Start()
	{
		playerAttack.currentSpell = availableSpells[currentSpellIndex];
	}

	private void Update()
	{
		HandleKnockbackState();

		HandleInput();

		if (CanReceiveInput())
		{
			if (rollPressed && rollCount < maxRolls && !isOnCooldown && IsGrounded())
			{
				StartCoroutine(PerformRoll());
			}

			if (attackPressed)
			{
				TryAttack();
			}
		}

		// Chặn move nếu đang tấn công
		if (!isAttacking && CanReceiveInput())
		{
			Move();
		}

		HandleJump();
		UpdateAnimator();

		attackPressed = false; // reset input mỗi frame
	}

	private void HandleInput()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		jumpPressed = Input.GetKeyDown(KeyCode.Space);
		rollPressed = Input.GetKeyDown(KeyCode.LeftShift);
		attackPressed = Input.GetMouseButtonDown(0);
	}

	private bool CanReceiveInput()
	{
		return canMove && !isRolling && !isKnockedBack;
	}

	private void Move()
	{
		playerRigidbody.velocity = new Vector2(horizontalInput * moveSpeed, playerRigidbody.velocity.y);

		if (horizontalInput != 0)
		{
			float newScaleX = Mathf.Sign(horizontalInput) * Mathf.Abs(transform.localScale.x);
			transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
		}
	}

	private void HandleJump()
	{
		if (jumpPressed)
		{
			if (IsGrounded())
			{
				Jump();
				jumpUsed = false;
			}
			else if (!jumpUsed)
			{
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

		if (playerAnimator != null && playerAttack.currentSpell != null)
		{
			string trigger = playerAttack.currentSpell.animationTrigger;
			Debug.Log($"[Attack] Trigger animation: {trigger}");
			playerAnimator.SetTrigger(trigger);
		}

		// Đợi animation kết thúc
		yield return new WaitForSeconds(0.8f); // Đặt thời gian bằng độ dài animation

		canMove = true;
		isAttacking = false;

		Debug.Log("[Attack] Attack finished, player can move again.");
	}

	// Hàm này sẽ được gọi từ Animation Event
	public void PerformAttack()
	{
		if (playerAttack != null)
		{
			playerAttack.PerformAttack();
		}
	}

	private void SwitchSpell(int index)
	{
		if (index >= 0 && index < availableSpells.Length)
		{
			currentSpellIndex = index;
			playerAttack.currentSpell = availableSpells[index];
		}
	}

	private void UpdateAnimator()
	{
		if (!canMove) return;

		var currentScale = transform.localScale;

		if (playerRigidbody.velocity.x < 0)
		{
			transform.localScale = new Vector3(-1f * Mathf.Abs(currentScale.x),
				currentScale.y, currentScale.z);
		}
		else if (playerRigidbody.velocity.x > 0)
		{
			transform.localScale = new Vector3(1f * Mathf.Abs(currentScale.x),
				currentScale.y, currentScale.z);
		}

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
