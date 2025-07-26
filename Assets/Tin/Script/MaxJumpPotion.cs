using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MaxJumpPotion : MonoBehaviour
{
    [SerializeField] private float boostedJumpForce = 14f;
    [SerializeField] private float boostDuration = 5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI boostText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.StartCoroutine(player.ApplyJumpBoost(boostedJumpForce, boostDuration, boostText));
            Destroy(gameObject);
        }
    }
}