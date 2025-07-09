using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] private GameObject spellPrefab;
	[SerializeField] private Transform firePoint;
	[SerializeField] private float spellSpeed = 10f;

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) // Chuột trái
		{
			ShootSpell();
		}
	}

	void ShootSpell()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 direction = (mousePos - firePoint.position).normalized;

		GameObject spell = Instantiate(spellPrefab, firePoint.position, Quaternion.identity);
		Rigidbody2D rb = spell.GetComponent<Rigidbody2D>();
		rb.velocity = direction * spellSpeed;
	}
}
