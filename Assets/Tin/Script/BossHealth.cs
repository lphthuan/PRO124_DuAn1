using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossHealth : MonoBehaviour, IDamageable
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

    [Header("Shield Regen Settings")]
    [SerializeField] private float shieldRegenDelay = 5f;

    private bool hasStartedRegen = false;

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

    public void TakeDamage(float damage, GameObject source)
    {
        if (currentShield > 0)
        {
            if (source.CompareTag("WindSpell"))
            {
                TakeShield(50f);
                return;
            }
            return;
        }


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

    public void TakeShield(float amount)
    {
        currentShield -= amount;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
        UpdateUI();

        // Khi giáp bằng 0 và chưa đếm hồi lại → bắt đầu đếm
        if (Mathf.Approximately(currentShield, 0f) && !hasStartedRegen)
        {
            hasStartedRegen = true;
            StartCoroutine(RegenShieldAfterDelay());
        }

        if (currentShield <= 0f)
        {
            ShieldSlider.gameObject.SetActive(false);
            if (shieldText != null)
                shieldText.gameObject.SetActive(false);

            healthSlider.gameObject.SetActive(true);
            if (healthText != null)
                healthText.gameObject.SetActive(true);
        }
    }

    private IEnumerator RegenShieldAfterDelay()
    {
        yield return new WaitForSeconds(shieldRegenDelay);

        currentShield = maxShield;
        hasStartedRegen = false;
        UpdateUI();

        // Hiện shield
        if (ShieldSlider != null)
            ShieldSlider.gameObject.SetActive(true);

        if (shieldText != null)
            shieldText.gameObject.SetActive(true);

        // Ẩn health
        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);

        if (healthText != null)
            healthText.gameObject.SetActive(false);
    }


    private void UpdateUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (healthFillImage != null)
            healthFillImage.fillAmount = currentHealth / maxHealth;

        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";

        if (ShieldSlider != null)
        {
            ShieldSlider.value = currentShield;
            ShieldSlider.gameObject.SetActive(currentShield > 0);
        }

        if (ShieldFillImage != null)
            ShieldFillImage.fillAmount = currentShield / maxShield;

        if (shieldText != null)
        {
            shieldText.text = $"{Mathf.CeilToInt(currentShield)} / {Mathf.CeilToInt(maxShield)}";
            shieldText.gameObject.SetActive(currentShield > 0);
        }

        if (currentShield > 0)
        {
            if (healthSlider != null)
                healthSlider.gameObject.SetActive(false);

            if (healthText != null)
                healthText.gameObject.SetActive(false);
        }

    }
}
