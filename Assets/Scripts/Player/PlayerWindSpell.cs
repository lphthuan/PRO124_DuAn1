using UnityEngine;

public class PlayerWindSpell : MonoBehaviour
{
	[SerializeField] private float knockbackForce = 10f;
	private Vector2 windDirection;

	public void SetDirection(Vector2 direction)
	{
		windDirection = direction.normalized;
		windDirection.y = 0f; // Không đẩy lên
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		IKnockbackable knockbackTarget = other.GetComponent<IKnockbackable>();
		if (knockbackTarget != null)
		{
			knockbackTarget.ApplyKnockback(windDirection, knockbackForce);
		}
	}
}
