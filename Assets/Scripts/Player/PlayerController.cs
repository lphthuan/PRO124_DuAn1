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
	private bool isAttacking = false; // kiểm soát trạng thái tấn công
	private bool attackPressed = false; // input bấm chuột

	// Input values
	private float horizontalInput;
	private bool jumpPressed;
	private bool rollPressed;

	private void Start()
	{
		playerAttack.currentSpell = availableSpells[currentSpellIndex];

	}

	void Update()
	{
		HandleKnockbackState();

		if (CanReceiveInput())
		{
			HandleInput();
			Move();
			HandleJump();

			// Điều kiện để được roll
			if (rollPressed && rollCount < maxRolls && !isOnCooldown && IsGrounded())
			{
				StartCoroutine(PerformRoll());
			}
		}

		if (attackPressed)
		{
			TryAttack(); // Gọi riêng phần tấn công
		}


		UpdateAnimator();
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

		isAttacking = true;

		if (playerAnimator != null && playerAttack.currentSpell != null)
		{
			playerAnimator.SetTrigger(playerAttack.currentSpell.animationTrigger);
		}
	}


	public void PerformAttack()
	{
		if (playerAttack != null)
		{
			playerAttack.PerformAttack();
		}
		isAttacking = false;
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

		if (Mathf.Abs(playerRigidbody.velocity.x) > 0.1f)
		{
			playerAnimator.SetBool("IsMove", true);
		}
		else
		{
			playerAnimator.SetBool("IsMove", false);
		}

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
		playerAnimator.Play("Player_Idle"); // hoặc animation idle default của bạn
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

		// Nếu đạt giới hạn roll → bắt đầu cooldown
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
