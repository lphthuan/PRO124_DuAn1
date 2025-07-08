using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))] //tự thêm components
public class PlayerController : MonoBehaviour
{
	[Header("Movement Settings")]
	[SerializeField] public float moveSpeed = 7f;
	[SerializeField] public float jumpForce = 8f;

	[Header("Components")]
	[SerializeField] private Rigidbody2D playerRigidbody;
	[SerializeField] private Animator playerAnimator;
	[SerializeField] private BoxCollider2D playerCollider;
	[SerializeField] private LayerMask terrainLayer;

	private bool canMove = true;
	private bool jumpUsed = false;

	// Input values
	private float horizontalInput;
	private bool jumpPressed;

	void Update()
	{
		if (canMove)
		{
			HandleInput();
			Move();
			HandleJump();
		}

		UpdateAnimator();
	}

	// Tách input – dễ nâng cấp sang tay cầm hoặc mobile
	private void HandleInput()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		jumpPressed = Input.GetKeyDown(KeyCode.Space);
	}

	// Di chuyển trái/phải
	private void Move()
	{
		playerRigidbody.velocity = new Vector2(horizontalInput * moveSpeed, playerRigidbody.velocity.y);

		if (horizontalInput != 0)
		{
			float newScaleX = Mathf.Sign(horizontalInput) * Mathf.Abs(transform.localScale.x);
			transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);
		}
	}

	// Xử lý nhảy & double jump
	private void HandleJump()
	{
		if (jumpPressed)
		{
			if (IsGrounded())
			{
				Jump();
				jumpUsed = false; // reset air jump
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

	// Giữ nguyên như yêu cầu
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

	// Cho phép control từ bên ngoài
	public void SetCanMove(bool value)
	{
		canMove = value;
	}
}
