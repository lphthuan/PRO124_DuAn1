using UnityEngine;

public class PlayerWindSpell : MonoBehaviour
{
	[SerializeField] private float knockbackForce = 10f;
	private Vector2 windDirection;

    private bool hasHit = false;

    public void SetDirection(Vector2 direction)
	{
		windDirection = direction.normalized;
		windDirection.y = 0f; // Chỉ đẩy ngang, không hất lên
	}

	private void Start()
	{
		Destroy(gameObject, 2f);
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // Gây knockback nếu có IKnockbackable
        IKnockbackable knockbackTarget = other.GetComponent<IKnockbackable>();
        if (knockbackTarget != null)
        {
            knockbackTarget.ApplyKnockback(windDirection, knockbackForce);
        }

        if (!hasHit && (other.CompareTag("Enemy") || other.CompareTag("Boss")))
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }

}
