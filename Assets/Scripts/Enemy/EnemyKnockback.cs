using UnityEngine;

public class EnemyKnockback : MonoBehaviour, IKnockbackable
{
	[SerializeField] private float knockbackForce = 8f;
	[SerializeField] private float kinematicPushDistance = 1.2f;
	[SerializeField] private float stunDuration = 0.4f;

	private Rigidbody2D rb;
	private bool isStunned = false;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	public void ApplyKnockback(Vector2 direction, float force)
	{
		if (isStunned || rb == null) return;

		// Chỉ đẩy ngang: loại bỏ lực hất lên
		direction.y = 0f;
		direction.Normalize();

		if (rb.bodyType == RigidbodyType2D.Dynamic)
		{
			rb.velocity = Vector2.zero;
			rb.AddForce(direction * force, ForceMode2D.Impulse);
		}
		else if (rb.bodyType == RigidbodyType2D.Kinematic)
		{
			Vector3 push = direction * kinematicPushDistance;
			transform.position += push;
		}

		StartCoroutine(StunForSeconds(stunDuration));
	}

	private System.Collections.IEnumerator StunForSeconds(float duration)
	{
		isStunned = true;
		yield return new WaitForSeconds(duration);
		isStunned = false;
	}
}
