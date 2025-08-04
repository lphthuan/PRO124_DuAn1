using UnityEngine;
using Cinemachine;

public class CamGiaiDo : MonoBehaviour
{
    public enum ZoneType { EnterPuzzleZone, ExitPuzzleZone }
    public ZoneType zoneType;

    [Header("Camera để bật (CamGĐ hoặc CamFL)")]
    public GameObject cameraToEnable;

    [Header("Camera để tắt (CamFL hoặc CamGĐ)")]
    public GameObject cameraToDisable;

    [Header("Virtual Camera theo Player")]
    public CinemachineVirtualCamera camFL;

    [Header("Player")]
    public GameObject player;

    private PlayerHealth playerHealth;

    private static CamGiaiDo activePuzzleZone = null;
    private bool hasResetAfterDeath = false;

    private void Start()
    {
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogWarning("CamGiaiDo: Player chưa được gán!");
        }
    }

    private void Update()
    {
        if (activePuzzleZone == this && playerHealth != null)
        {
            // Nếu chết trong puzzle zone → reset camera
            if (playerHealth.currentHealth <= 0 && !hasResetAfterDeath)
            {
                ResetToFollowCamera();
                hasResetAfterDeath = true;
            }

            // Cho phép reset lại nếu player sống lại
            if (playerHealth.currentHealth > 0 && hasResetAfterDeath)
            {
                hasResetAfterDeath = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (cameraToEnable != null)
            cameraToEnable.SetActive(true);

        if (cameraToDisable != null)
            cameraToDisable.SetActive(false);

        if (zoneType == ZoneType.EnterPuzzleZone)
        {
            activePuzzleZone = this;

            if (camFL != null)
            {
                camFL.Priority = 0;
                camFL.gameObject.SetActive(false); // Tắt camFL khi vào puzzle
            }
        }
        else if (zoneType == ZoneType.ExitPuzzleZone)
        {
            activePuzzleZone = null;

            if (camFL != null)
            {
                camFL.Priority = 10;
                camFL.gameObject.SetActive(true); // Bật lại camFL khi thoát
            }
        }
    }

    private void ResetToFollowCamera()
    {
        if (cameraToEnable != null)
            cameraToEnable.SetActive(false); // Tắt CamGĐ

        if (cameraToDisable != null)
            cameraToDisable.SetActive(true); // Bật CamFL

        if (camFL != null)
        {
            camFL.Priority = 10;
            camFL.gameObject.SetActive(true); // Đảm bảo camFL bật
        }

        activePuzzleZone = null;
    }

    // Gọi từ CheckpointManager sau khi respawn
    public void ForceResetCamera()
    {
        ResetToFollowCamera();
        hasResetAfterDeath = false;
    }

    public static CamGiaiDo GetActivePuzzleZone()
    {
        return activePuzzleZone;
    }
}
