using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MiniBossHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 1000f;
    public float currentHealth;

    [Header("UI - Sliders")]
    public Slider healthSlider;
    public Image healthFillImage;

    [Header("UI - Text")]
    [SerializeField] private TMP_Text healthText;

    public delegate void BossDeathEvent();
    public event BossDeathEvent OnBossDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
            healthSlider.maxValue = maxHealth;

        UpdateUI();
    }

    public void TakeDamage(float damage, GameObject source)
    {
        currentHealth -= damage;
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
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }
}