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
        soulText.text = soulCount.ToString();
    }
}
