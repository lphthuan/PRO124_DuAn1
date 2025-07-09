using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightningSpell : MonoBehaviour
{
	[SerializeField] private float lifetime = 2f;
	[SerializeField] private int damage = 1;

	void Start()
	{
		Destroy(gameObject, lifetime); // Tự huỷ sau 2s
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Enemy"))
		{
			// Gây sát thương nếu enemy có health
			// other.GetComponent<EnemyHealth>()?.TakeDamage(damage);
			Destroy(gameObject);
		}
	}
}
