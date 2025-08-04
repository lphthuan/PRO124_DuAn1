using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Transform currentCheckpoint;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        // Vô hiệu hóa checkpoint cũ
        if (currentCheckpoint != null)
        {
            var old = currentCheckpoint.GetComponent<Checkpoint>();
            if (old != null)
                old.Deactivate();
        }

        currentCheckpoint = newCheckpoint;
    }

    public Vector3 GetRespawnPosition()
    {
        if (currentCheckpoint != null)
            return currentCheckpoint.position;

        Debug.LogWarning("[CheckpointManager] Không có Checkpoint! Trả về Vector3.zero");
        return Vector3.zero;
    }

    public bool HasCheckpoint()
    {
        return currentCheckpoint != null;
    }

    // GỌI SAU KHI PLAYER CHẾT & HỒI SINH
    public void HandleCameraResetOnRespawn()
    {
        if (CamGiaiDo.GetActivePuzzleZone() != null)
        {
            CamGiaiDo.GetActivePuzzleZone().ForceResetCamera();
        }
    }
}
