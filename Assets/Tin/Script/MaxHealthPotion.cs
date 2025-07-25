using UnityEngine;

public class MaxHealthPotion : MonoBehaviour
{
    public int bonusMaxHealth = 1000;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.IncreaseMaxHealth(bonusMaxHealth);
            Destroy(gameObject); // Xoá sau khi nhặt
        }
    }
}
