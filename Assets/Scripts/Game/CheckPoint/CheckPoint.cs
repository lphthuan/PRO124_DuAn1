using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	private bool isActive = false;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (isActive) return;

		if (other.CompareTag("Player"))
		{
			Debug.Log($"[Checkpoint] Active at: {transform.position}");
			CheckpointManager.Instance.SetCheckpoint(transform);
			ActivateCheckpoint();
		}
	}

	private void ActivateCheckpoint()
	{
		isActive = true;

		GetComponent<SpriteRenderer>().color = Color.green;
	}

	public void Deactivate()
	{
		isActive = false;
		GetComponent<SpriteRenderer>().color = Color.white;
	}
}
