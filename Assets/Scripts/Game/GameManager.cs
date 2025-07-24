using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("World Objects")]
	[SerializeField] private GameObject materialWorld;
	[SerializeField] private GameObject spiritWorld;

	private bool isInSpiritWorld = false;

	void Start()
	{
		// Đảm bảo ban đầu chỉ thế giới vật chất hiện
		materialWorld.SetActive(true);
		spiritWorld.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			ToggleWorld();
		}
	}

	private void ToggleWorld()
	{
		isInSpiritWorld = !isInSpiritWorld;

		materialWorld.SetActive(!isInSpiritWorld);
		spiritWorld.SetActive(isInSpiritWorld);

		Debug.Log($"[World Switch] Chuyển sang {(isInSpiritWorld ? "THẾ GIỚI LINH HỒN 👻" : "THẾ GIỚI VẬT CHẤT 🏔️")}");
	}
}
