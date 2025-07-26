using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MaxSpeedPotion : MonoBehaviour
{
    [SerializeField] private float speedBoost = 14f;
    [SerializeField] private float boostDuration = 5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI boostText; // Gán từ Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // Chuyển coroutine sang Player để vẫn chạy dù bị destroy
            player.StartCoroutine(player.ApplySpeedBoost(speedBoost, boostDuration, boostText));

            // Hủy SpeedPotion ngay sau khi dùng
            Destroy(gameObject);
        }
    }
}