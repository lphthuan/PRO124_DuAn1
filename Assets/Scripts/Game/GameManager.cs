using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[Header("World Objects")]
	[SerializeField] private GameObject materialWorld;
	[SerializeField] private GameObject spiritWorld;

	[Header("UI World Indicator")]
	[SerializeField] private Image worldIcon;
	[SerializeField] private Sprite spriteMaterialWorld;
	[SerializeField] private Sprite spriteSpiritWorld;

	private bool isInSpiritWorld = false;

	void Start()
	{
		// Khởi đầu ở thế giới vật chất
		materialWorld.SetActive(true);
		spiritWorld.SetActive(false);
		UpdateWorldIcon();
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

		UpdateWorldIcon();
	}

	private void UpdateWorldIcon()
	{
		if (worldIcon != null)
		{
			worldIcon.sprite = isInSpiritWorld ? spriteSpiritWorld : spriteMaterialWorld;
		}
	}
}
