using System.Collections;
using TMPro;
using UnityEngine;

public class IncreaseDamage : MonoBehaviour
{
    [Header("Damage Boost Settings")]
    [SerializeField] private float bonusDamage = 5f;
    [SerializeField] private float boostDuration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAttack playerAttack = other.GetComponent<PlayerAttack>();
            if (playerAttack != null)
            {
                playerAttack.BoostDamage(bonusDamage, boostDuration);
                Debug.Log($"🟢 Boost damage: +{bonusDamage} trong {boostDuration} giây");
            }

            Destroy(gameObject); // Xoá potion sau khi dùng
        }
    }
}
