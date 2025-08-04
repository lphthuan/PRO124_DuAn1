using UnityEngine;

public class WindTrigger : MonoBehaviour
{
	[Header("Target GameObjects")]
	[SerializeField] private GameObject objectA;
	[SerializeField] private GameObject objectB;

	[Header("Animation")]
	[SerializeField] private Animator animator;
	[SerializeField] private string animationTrigger = "Spin";

	[Header("Trigger Settings")]
	[SerializeField] private string triggeringTag = "WindSpell";

	private bool state = false;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag(triggeringTag)) return;

		state = !state;

		// Cập nhật GameObject
		if (objectA != null) objectA.SetActive(!state);
		if (objectB != null) objectB.SetActive(state);

		// Trigger animation
		if (animator != null && !string.IsNullOrEmpty(animationTrigger))
		{
			animator.SetTrigger(animationTrigger);
		}

		Debug.Log($"[WindTrigger] Toggled! State: {(state ? "B On" : "A On")}");
	}
}
