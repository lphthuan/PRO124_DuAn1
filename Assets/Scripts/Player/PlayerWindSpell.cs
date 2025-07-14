using UnityEngine;

public class PlayerWindSpell : MonoBehaviour
{
	[SerializeField] private float knockbackForce = 10f;
	private Vector2 windDirection;

	public void SetDirection(Vector2 direction)
	{
		windDirection = direction.normalized;
		windDirection.y = 0f; // Chỉ đẩy ngang, không hất lên
	}

	private void Start()
	{
		Destroy(gameObject, 2f); // Tự hủy sau 2 giây
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		IKnockbackable knockbackTarget = other.GetComponent<IKnockbackable>();
		if (knockbackTarget != null)
		{
			knockbackTarget.ApplyKnockback(windDirection, knockbackForce);
		}

		// Nếu chạm vào Enemy thì hủy luồng gió luôn
		if (other.CompareTag("Enemy"))
		{
			Destroy(gameObject);
		}
	}
}
