using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
	[Header("Objects to Activate On Death")]
	[SerializeField] private GameObject[] objectsToActivate;

	/// <summary>
	/// call at animation event when the enemy dies.
	/// </summary>
	public void ActivateOnDeath()
	{
		foreach (GameObject obj in objectsToActivate)
		{
			if (obj != null)
				obj.SetActive(true);
		}

		Debug.Log($"[DeathTrigger] On {objectsToActivate.Length}");
	}
}
