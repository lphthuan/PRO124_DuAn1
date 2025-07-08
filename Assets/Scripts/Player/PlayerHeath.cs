using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeath : MonoBehaviour
{
    [Header("Player Health Settings")]
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] public int currentHealth;
    
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Text healthText;
    [SerializeField] private Animator playerAnimator;

    [SerializeField] private PlayerController playerController;
    //[SerializeField] private PlayerAttack playerAttack;

    private bool isDead = false; 


    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
	}

    void Update()
    {
        UpdateHealthBar();

		if (Input.GetKeyDown(KeyCode.R))
		{
			TakeDamage(30);
		}
	}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(40);
        }
	}


    public void TakeDamage(int damage)
    {
        if(isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        UpdateHealthBar();

		if (playerAnimator != null && currentHealth > 0 && damage > 20)
		{
			playerAnimator.SetTrigger("IsHurt");
		}
	}

    public void UpdateHealthBar()
    {
        float fillAmount = (float)currentHealth / maxHealth;
        healthBarFill.fillAmount = fillAmount;

        healthText.text = currentHealth + " / " + maxHealth;
    }

	private IEnumerator HandleDeath()
	{
		if (playerAnimator != null)
		{
			playerAnimator.SetTrigger("IsDead");
		}

        // Vô hiệu hóa các thành phần điều khiển và tấn công của người chơi
        if (playerController != null) playerController.enabled = false;
        //if (playerAttack != null) playerAttack.enabled = false;

        yield return new WaitForSeconds(1.5f);

		Respawn();
	}

	private void Respawn()
	{
        if (playerController != null) playerController.enabled = true;
        //if (playerAttack != null) playerAttack.enabled = true;

        currentHealth = maxHealth;
		isDead = false; // cho phép chết lại sau khi hồi sinh

		UpdateHealthBar(); // cập nhật lại thanh máu
	}
}
