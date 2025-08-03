using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CamGiaiDo : MonoBehaviour
{
    [Header("Camera sẽ được bật khi player vào vùng này")]
    public GameObject cameraToEnable;

    [Header("Camera sẽ bị tắt (bình thường là camera cũ)")]
    public GameObject cameraToDisable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (cameraToDisable != null)
            cameraToDisable.SetActive(false);

        if (cameraToEnable != null)
            cameraToEnable.SetActive(true);
    }
}
