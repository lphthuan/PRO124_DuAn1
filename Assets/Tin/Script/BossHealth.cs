using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 1000f;
    public float currentHealth;
    public float maxShield = 100f;
    public float currentShield;

    [Header("UI - Sliders")]
    public Slider healthSlider;
    public Image healthFillImage;
    public Slider ShieldSlider;
    public Image ShieldFillImage;

    [Header("UI - Text")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text shieldText;

    public delegate void BossDeathEvent();
    public event BossDeathEvent OnBossDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;

        if (healthSlider != null)
            healthSlider.maxValue = maxHealth;

        if (ShieldSlider != null)
            ShieldSlider.maxValue = maxShield;

        UpdateUI();
    }

    public void TakeShield(float amount)
    {
        currentShield -= amount;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
        UpdateUI();

        if (Mathf.Approximately(currentShield, 0f))
        {
            ShieldSlider.gameObject.SetActive(false);
            if (shieldText != null)
                shieldText.gameObject.SetActive(false);

            healthSlider.gameObject.SetActive(true);
            if (healthText != null)
                healthText.gameObject.SetActive(true);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            OnBossDead?.Invoke();

            if (healthSlider != null)
                Destroy(healthSlider.gameObject);

            if (healthText != null)
                Destroy(healthText.gameObject);
        }
    }

    private void UpdateUI()
    {
        // Health
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (healthFillImage != null)
            healthFillImage.fillAmount = currentHealth / maxHealth;

        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";

        // Shield
        if (ShieldSlider != null)
        {
            ShieldSlider.value = currentShield;
            ShieldSlider.gameObject.SetActive(currentShield > 0); // ẩn khi hết khiên
        }

        if (ShieldFillImage != null)
            ShieldFillImage.fillAmount = currentShield / maxShield;

        if (shieldText != null)
        {
            shieldText.text = $"{Mathf.CeilToInt(currentShield)} / {Mathf.CeilToInt(maxShield)}";
            shieldText.gameObject.SetActive(currentShield > 0); // ẩn khi hết khiên
        }
    }
}
