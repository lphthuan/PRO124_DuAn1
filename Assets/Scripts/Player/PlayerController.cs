using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] public float moveSpeed = 7f;
	[SerializeField] public float jumpForce = 8f;

	[SerializeField] private Rigidbody2D playerRigidbody;
	[SerializeField] private Animator playerAnimator;
	[SerializeField] private BoxCollider2D playerCollider;
	[SerializeField] private Transform playerTransform;
	[SerializeField] private LayerMask terrainLayer;

	private bool canMove = true;
	private bool jumpCheck = false;

	void Update()
	{
		if (canMove)
		{
			Movement();
		}

		UpdateAnimator();
	}

	private void Movement()
	{
		if (!canMove) return;
		float horizontal = Input.GetAxis("Horizontal");
		playerRigidbody.velocity = new Vector2(horizontal * moveSpeed, playerRigidbody.velocity.y);

		if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
		{
			playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
		}
		if (IsGrounded() == true)
		{
			jumpCheck = true;
		}
		if (IsGrounded() == false && jumpCheck == true && Input.GetKeyDown(KeyCode.Space))
		{
			playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
			jumpCheck = false;
		}
	}

	private bool IsGrounded()
	{
		return Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f,
			Vector2.down, 0.1f, terrainLayer);
	}

	private void UpdateAnimator()
	{
		if (!canMove) return;
		var currentScale = playerTransform.localScale;
		if (playerRigidbody.velocity.x < 0)
		{
			playerTransform.localScale = new Vector3(-1f * Mathf.Abs(currentScale.x),
				currentScale.y, currentScale.z);
		}
		else if (playerRigidbody.velocity.x > 0)
		{
			playerTransform.localScale = new Vector3(1f * Mathf.Abs(currentScale.x),
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
}
