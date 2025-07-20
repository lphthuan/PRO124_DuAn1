using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthSlider;
    public Image healthFillImage;

    public delegate void BossDeathEvent();
    public event BossDeathEvent OnBossDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
            healthSlider.maxValue = maxHealth; 

        UpdateUI();
    }

    public void TakeDamage(float amount) //chat gpt
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); 
        UpdateUI();

        if (currentHealth <= 0)
        {
            OnBossDead?.Invoke();

            if (healthSlider != null)
                Destroy(healthSlider.gameObject); 
        }
    }


    private void UpdateUI() //chst gpt
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth; 

        if (healthFillImage != null)
            healthFillImage.fillAmount = currentHealth / maxHealth;
    }
}
