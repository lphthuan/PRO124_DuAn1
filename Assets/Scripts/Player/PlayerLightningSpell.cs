using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class PlayerLightningSpell : MonoBehaviour
{
	[SerializeField] private float lifetime = 2f;
    //[SerializeField] private int damage = 1;

    private bool hasHit = false;

    void Start()
	{
		Destroy(gameObject, lifetime); // Tự huỷ sau 2s
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return; // ⛔ Nếu đã va chạm, bỏ qua

        // Gây sát thương nếu đối tượng có IDamageable
        /*IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            float damage = PlayerAttack.Instance.GetDamage(); // Lấy damage từ Player
            target.TakeDamage(damage, gameObject); // Truyền damage và source
        }*/

        // Nếu trúng Enemy hoặc Boss → hủy spell
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            hasHit = true; // ✅ Đánh dấu đã trúng
            Destroy(gameObject); // ✅ Huỷ spell sau 1 hit
        }
    }

}
