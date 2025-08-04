using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoulUIManager : MonoBehaviour
{
    public static SoulUIManager instance;

    public TextMeshProUGUI soulText;
    public Image soulIcon;

    private int soulCount = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddSoul(int amount)
    {
        soulCount += amount;
        UpdateSoulUI();
    }

    public bool SpendSoul(int cost)
    {
        if (soulCount >= cost)
        {
            soulCount -= cost;
            UpdateSoulUI();
            return true; // Mua thành công
        }
        else
        {
            Debug.Log("Not enough souls!");
            return false;
        }
    }

    private void UpdateSoulUI()
    {
        soulText.text = $"{soulCount} Soul";
    }

    public int GetCurrentSoul()
    {
        return soulCount;
    }
}
