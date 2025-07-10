using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[Header("Attack Settings")]
	[SerializeField] private Transform firePoint;
	[SerializeField] private float spellSpeed = 12f;
	[SerializeField] private float attackAngle = 120f;

	public SpellData currentSpell;

	// Kiểm tra xem chuột có trong vùng bắn hợp lệ không
	public bool IsValidAttackAngle()
	{
		if (currentSpell == null) return false;

		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 direction = (mousePos - firePoint.position);
		direction.z = 0f;
		Vector2 dirNormalized = direction.normalized;

		bool facingRight = transform.localScale.x > 0;
		Vector2 facing = facingRight ? Vector2.right : Vector2.left;
		float angle = Vector2.Angle(facing, dirNormalized);

		Debug.Log($"[Attack] Facing: {(facingRight ? "Right" : "Left")} | Angle to mouse: {angle}");

		return angle <= attackAngle / 2f;
	}

	public void PerformAttack()
	{
		if (currentSpell == null) return;

		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 direction = (mousePos - firePoint.position);
		direction.z = 0f;
		Vector2 dirNormalized = direction.normalized;

		float zRotation = Mathf.Atan2(dirNormalized.y, dirNormalized.x) * Mathf.Rad2Deg;

		GameObject spell = Instantiate(currentSpell.spellPrefab, firePoint.position, Quaternion.Euler(0, 0, zRotation));
		Rigidbody2D rb = spell.GetComponent<Rigidbody2D>();
		rb.velocity = dirNormalized * spellSpeed;

		Debug.Log($"[Attack] Spell casted at angle: {zRotation}°");
	}
}
