using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    [Header("Shake settings")]
    public ShakeData shakeData; // Kéo ShakeData preset vào đây
 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra va chạm với object ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Gọi rung camera
            if (CameraShaker.Instance != null && shakeData != null)
            {
                CameraShaker.Instance.Shake(shakeData);
            }
        }
    }
}
