using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 3000f;
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
    [SerializeField] private float healthLostThresholdToRegenShield = 1000f;

    private float healthLostSinceShieldBreak = 0f;

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
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        // Boss chết thì không làm gì thêm
        if (currentHealth <= 0)
        {
            OnBossDead?.Invoke();

            if (healthSlider != null)
                Destroy(healthSlider.gameObject);

            if (healthText != null)
                Destroy(healthText.gameObject);

            return; // ⛔ Dừng lại luôn
        }

        // Boss chưa chết → cộng dồn máu mất
        healthLostSinceShieldBreak += damage;

        if (healthLostSinceShieldBreak >= healthLostThresholdToRegenShield)
        {
            RegenShieldFromHealthLoss();
        }
    }


    public void TakeShield(float amount)
    {
        currentShield -= amount;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
        UpdateUI();

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

    private void RegenShieldFromHealthLoss()
    {
        currentShield = maxShield;
        healthLostSinceShieldBreak = 0f;

        UpdateUI();

        if (ShieldSlider != null)
            ShieldSlider.gameObject.SetActive(true);

        if (shieldText != null)
            shieldText.gameObject.SetActive(true);

        if (healthSlider != null)
            healthSlider.gameObject.SetActive(false);

        if (healthText != null)
            healthText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (currentShield > 0)
        {
            if (collision.CompareTag("WindSpell"))
            {
                TakeShield(50f);
                Destroy(collision.gameObject);
            }
            return;
        }

        if (collision.CompareTag("PlayerBullet"))
        {
            float damage = PlayerAttack.Instance.GetDamage();
            TakeDamage(damage, collision.gameObject);

            Destroy(collision.gameObject);
        }
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
