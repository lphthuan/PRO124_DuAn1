using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	private bool isActive = false;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (isActive) return;

		if (other.CompareTag("Player"))
		{
			Debug.Log($"[Checkpoint] Kích hoạt tại: {transform.position}");
			CheckpointManager.Instance.SetCheckpoint(transform);
			ActivateCheckpoint();
		}
	}

	private void ActivateCheckpoint()
	{
		isActive = true;
		// Hiệu ứng hoặc đổi màu ở đây (nếu muốn)
		// ví dụ: GetComponent<SpriteRenderer>().color = Color.green;
	}

	public void Deactivate()
	{
		isActive = false;
		// GetComponent<SpriteRenderer>().color = Color.white;
	}
}
