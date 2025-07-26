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
        // Gây knockback nếu có IKnockbackable
        IKnockbackable knockbackTarget = other.GetComponent<IKnockbackable>();
        if (knockbackTarget != null)
        {
            knockbackTarget.ApplyKnockback(windDirection, knockbackForce);
        }

        // Gây damage nếu có IDamageable
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            float damage = PlayerAttack.Instance.GetDamage(); // Lấy damage từ Player
            target.TakeDamage(damage, gameObject); // Truyền damage và source
        }

        // Hủy luồng gió nếu chạm Enemy (hoặc sau khi gây damage)
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }

}
