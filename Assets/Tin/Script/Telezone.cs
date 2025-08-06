using UnityEngine;
using UnityEngine.SceneManagement;

public class Telezone : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			SceneManager.LoadScene("Map2");
		}
	}
}
