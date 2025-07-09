using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[Header("Attack Settings")]
	[SerializeField] private GameObject spellPrefab;
	[SerializeField] private Transform firePoint;
	[SerializeField] private float spellSpeed = 12f;
	[SerializeField] private float attackAngle = 120f;
	[SerializeField] private Animator playerAnimator;

	public void PerformAttack()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 direction = (mousePos - firePoint.position);
		direction.z = 0f;
		Vector2 dirNormalized = direction.normalized;

		bool facingRight = transform.localScale.x > 0;
		Vector2 facing = facingRight ? Vector2.right : Vector2.left;
		float angle = Vector2.Angle(facing, dirNormalized);

		if (angle <= attackAngle / 2f)
		{
			// ✨ Tính góc xoay
			float zRotation = Mathf.Atan2(dirNormalized.y, dirNormalized.x) * Mathf.Rad2Deg;

			// 🌀 Tạo spell xoay đúng hướng
			GameObject spell = Instantiate(spellPrefab, firePoint.position, Quaternion.Euler(0, 0, zRotation));

			Rigidbody2D rb = spell.GetComponent<Rigidbody2D>();
			rb.velocity = dirNormalized * spellSpeed;

			// 🔥 Animation
			if (playerAnimator != null)
				playerAnimator.SetTrigger("IsShoot");
		}
	}

}
