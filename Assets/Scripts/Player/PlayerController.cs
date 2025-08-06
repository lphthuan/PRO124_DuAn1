using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
	[Header("Movement Settings")]
	[SerializeField] public float moveSpeed = 7f; // Player movement speed
	[SerializeField] public float jumpForce = 8f; // Player jump force

	[Header("Roll Settings")]
	[SerializeField] private float rollSpeed = 8f; // Speed during roll
	[SerializeField] private float rollDuration = 0.3f; // Duration of roll in seconds
	[SerializeField] private int maxRolls = 2; // Max consecutive rolls before cooldown
	[SerializeField] private float rollCooldown = 1.5f; // Cooldown after max rolls

	private bool isRolling = false; // Is player currently rolling
	private int rollCount = 0; // Number of rolls performed
	private bool isOnCooldown = false; // Is roll on cooldown

	[Header("Components")]
	[SerializeField] private Rigidbody2D playerRigidbody; // Main Rigidbody2D
	[SerializeField] private Animator playerAnimator; // Animator for player
	[SerializeField] private BoxCollider2D playerCollider; // Collider for ground check
	[SerializeField] private LayerMask terrainLayer; // LayerMask for ground detection
	[SerializeField] private PlayerAttack playerAttack; // Reference to PlayerAttack script
	[SerializeField] private SpellData[] availableSpells; // List of available spells
	private Rigidbody2D rb; // Cached Rigidbody2D
	private SpriteRenderer spriteRenderer; // SpriteRenderer for flipping

	// Shield system
	[SerializeField] private KeyCode shieldKey = KeyCode.F; // Key to activate shield
	private GameObject activeShield; // Current shield instance
	public int shieldSpellLevel = 0; // Level of shield skill
	public bool shieldCheck = false; // Is shield active
	public bool shieldHave = false; // Does player have shield
	[SerializeField] GameObject shieldSpell; // Shield prefab

	private int currentSpellIndex = 0; // Current selected spell index
	private bool canMove = true; // Can player move
	private bool jumpUsed = false; // Has double jump been used
	private bool isKnockedBack = false; // Is player being knocked back
	private float knockbackDuration = 0.3f; // Duration of knockback
	private float currentKnockbackTimer = 0f; // Knockback timer
	private bool isAttacking = false; // Is player attacking
	private Coroutine attackRoutine; // Attack coroutine

	// Spell idle state
	private bool isHoldingSpellIdle = false; // Is spell idle active
	private Coroutine spellIdleCoroutine; // Spell idle coroutine
	private float spellIdleDuration = 20f; // Spell idle duration

	// Input values
	private float horizontalInput;
	private bool jumpPressed;
	private bool rollPressed;
	private bool attackPressed;
	private bool switchSpellPressed;

	private float defaultMoveSpeed; // Default move speed
	private float defaultJumpForce; // Default jump force

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		playerAttack.currentSpell = availableSpells[currentSpellIndex];
		defaultMoveSpeed = moveSpeed;
		defaultJumpForce = jumpForce;
		SetAnimatorIdleState(); // Set initial idle state

		LoadShieldSkillState();
	}

	private void Update()
	{
		HandleKnockbackState();
		HandleInput();

		HandleSpellSwitch(); // Switch spell with Q

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

		// Shield activation
		if (Input.GetKeyDown(shieldKey) && CanReceiveInput())
		{
			if (IsParryUnlocked())
				TryCastShield();
			else
				Debug.Log("Parry skill is not unlocked!");
		}
	}

	public void SetMoveSpeed(float newSpeed)
	{
		moveSpeed = newSpeed;
	}

	public void SetJumpForce(float newJumpForce)
	{
		jumpForce = newJumpForce;
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
			PlayerAttack.Instance.UpdateSpellIcon(); // Update spell icon UI
		}
	}

	private void SwitchSpell(int index)
	{
		if (index >= 0 && index < availableSpells.Length)
		{
			currentSpellIndex = index;
			playerAttack.currentSpell = availableSpells[currentSpellIndex];
			SetAnimatorIdleState();
			Debug.Log($"[Spell] Switched to spell: {availableSpells[currentSpellIndex].name}");
		}
	}

	/// <summary>
	/// Set animator to idle state for the current spell.
	/// </summary>
	private void SetAnimatorIdleState()
	{
		if (playerAnimator != null && playerAttack.currentSpell != null)
		{
			int spellState = playerAttack.currentSpell.StateIntAnim;
			playerAnimator.SetInteger("State", spellState);
			Debug.Log($"[Idle] Switched to spell idle: {spellState}");

			isHoldingSpellIdle = true;

			// Cancel previous countdown if running
			if (spellIdleCoroutine != null) StopCoroutine(spellIdleCoroutine);
			spellIdleCoroutine = StartCoroutine(SpellIdleCountdown());
		}
	}

	private IEnumerator SpellIdleCountdown()
	{
		yield return new WaitForSeconds(spellIdleDuration);

		if (isHoldingSpellIdle)
		{
			playerAnimator.SetInteger("State", 0); // Return to default idle
			Debug.Log("[Idle] Spell idle timeout → returning to default idle (0)");
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
			Debug.Log("[Idle] Spell idle interrupted by action");
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

			// Flip sprite based on movement direction
			spriteRenderer.flipX = horizontalInput < 0;
		}
	}

	private void HandleJump()
	{
		if (jumpPressed)
		{
			// Prevent jumping while rolling, attacking, or knocked back
			if (isRolling || isAttacking || isKnockedBack) return;

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
		// BoxCast to check if player is on the ground
		return Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f,
			Vector2.down, 0.1f, terrainLayer);
	}

	private void TryAttack()
	{
		CancelSpellIdle();
		if (isAttacking || isRolling || !IsGrounded()) return;

		if (attackRoutine != null) StopCoroutine(attackRoutine);
		attackRoutine = StartCoroutine(AttackRoutine());
	}

	/// <summary>
	/// Handles attack animation and timing.
	/// </summary>
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

		// Reset trigger to prevent animation stuck
		if (playerAnimator != null && playerAttack.currentSpell != null)
		{
			playerAnimator.ResetTrigger(playerAttack.currentSpell.animationTrigger);
		}

		SetAnimatorIdleState(); // Return to corresponding idle
		canMove = true;
		isAttacking = false;
	}

	/// <summary>
	/// Called by animation event to perform the attack.
	/// </summary>
	public void PerformAttack()
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

	/// <summary>
	/// Handles player roll action and cooldown.
	/// </summary>
	private IEnumerator PerformRoll()
	{
		CancelSpellIdle();
		isRolling = true;
		canMove = false;
		rollCount++;

		float direction = spriteRenderer.flipX ? -1f : 1f;
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

	public IEnumerator ApplySpeedBoost(float boostSpeed, float duration, TextMeshProUGUI boostText)
	{
		float originalSpeed = moveSpeed;
		SetMoveSpeed(boostSpeed);

		Debug.Log($"Speed increased to {boostSpeed} for {duration} seconds");

		if (boostText != null)
			boostText.gameObject.SetActive(true);

		float timeLeft = duration;
		while (timeLeft > 0)
		{
			if (boostText != null)
				boostText.text = $"Speed Boost: {timeLeft:F1}s";

			timeLeft -= Time.deltaTime;
			yield return null;
		}

		SetMoveSpeed(originalSpeed);

		Debug.Log($"Speed boost ended, speed reset to: {originalSpeed}");

		if (boostText != null)
			boostText.gameObject.SetActive(false);
	}

	public IEnumerator ApplyJumpBoost(float boostedJump, float duration, TextMeshProUGUI boostText)
	{
		float originalJump = jumpForce;
		SetJumpForce(boostedJump);

		Debug.Log($"Jump force increased to {boostedJump} for {duration} seconds");

		if (boostText != null)
			boostText.gameObject.SetActive(true);

		float timeLeft = duration;
		while (timeLeft > 0)
		{
			if (boostText != null)
				boostText.text = $"Jump Boost: {timeLeft:F1}s";

			timeLeft -= Time.deltaTime;
			yield return null;
		}

		SetJumpForce(originalJump);

		Debug.Log($"Jump boost ended, jump force reset to: {originalJump}");

		if (boostText != null)
			boostText.gameObject.SetActive(false);
	}

	private void HandleKnockbackState()
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

	// Shield system

	private bool IsParryUnlocked()
	{
		return shieldSpellLevel > 0;
	}

	/// <summary>
	/// Try to cast shield if available.
	/// </summary>
	private void TryCastShield()
	{
		if (shieldSpellLevel == 0)
		{
			Debug.Log("[Shield] Shield not unlocked!");
			return;
		}

		if (shieldSpellLevel == 1 && shieldCheck == false)
		{
			if (activeShield != null)
			{
				Destroy(activeShield);
			}

			Vector3 shieldOffset = new Vector3(0, 0, 0);
			activeShield = Instantiate(shieldSpell, transform.position + shieldOffset, Quaternion.identity, transform);
			shieldCheck = true;
			shieldHave = true;
		}
	}

	private void LoadShieldSkillState()
	{
		if (PlayerPrefs.HasKey("ShieldSkillLevel"))
		{
			shieldSpellLevel = PlayerPrefs.GetInt("ShieldSkillLevel");
			shieldHave = shieldSpellLevel > 0;
		}
		else
		{
			shieldSpellLevel = 0;
			shieldHave = false;
		}
	}

	/// <summary>
	/// Coroutine to reset shield state after a delay.
	/// </summary>
	public IEnumerator StartShieldCountdown()
	{
		yield return new WaitForSeconds(0.5f);
		shieldHave = false;
		yield return new WaitForSeconds(5f);
		shieldCheck = false;
	}
}