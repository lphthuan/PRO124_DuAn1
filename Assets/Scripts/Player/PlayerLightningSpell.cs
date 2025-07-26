using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightningSpell : MonoBehaviour
{
	[SerializeField] private float lifetime = 2f;
	//[SerializeField] private int damage = 1;

	void Start()
	{
		Destroy(gameObject, lifetime); // Tự huỷ sau 2s
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Gây sát thương nếu đối tượng có IDamageable
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            float damage = PlayerAttack.Instance.GetDamage(); // Lấy damage từ Player
            target.TakeDamage(damage, gameObject); // Truyền damage và source
        }

        // Huỷ gameObject (spell) nếu trúng enemy
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            Destroy(gameObject);
        }
    }

}
