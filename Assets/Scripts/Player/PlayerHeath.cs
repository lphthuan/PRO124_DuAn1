using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
	[Header("Player Health Settings")]
	[SerializeField] private int maxHealth = 1000;
	[SerializeField] public int currentHealth;

	[SerializeField] private Image healthBarFill;
	[SerializeField] private Text healthText;
	[SerializeField] private Animator playerAnimator;

	[SerializeField] private PlayerController playerController;
	[SerializeField] private PlayerAttack playerAttack;

	[SerializeField] private float knockbackForce = 5f;

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
			TakeDamage(300);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		int damage = 0;

		switch (other.tag)
		{
			case "Enemy":
				damage = 30;
				break;
			case "EnemyBullet":
				damage = 70;
				break;
			case "CauLuaQuai2":
				damage = 250;
				break;
			case "Trap":
				damage = 100;
				break;
			case "Arrow":
				damage = 50;
				break;
			case "Explotion":
				damage = 150;
				break;
			default:
				return;
		}

		TakeDamage(damage);
		ApplyKnockback(other);
	}

	private void ApplyKnockback(Collider2D other)
	{
		if (playerController == null) return;

		Vector2 knockbackDirection = (transform.position - other.transform.position);
		if (knockbackDirection.sqrMagnitude < 0.001f)
		{
			knockbackDirection = new Vector2(Random.Range(-1f, 1f), 0.2f);
			if (knockbackDirection.x == 0) knockbackDirection.x = 1f;
		}

		knockbackDirection = knockbackDirection.normalized;
		knockbackDirection.y += 0.5f;
		knockbackDirection = knockbackDirection.normalized;

		playerController.ApplyKnockback(knockbackDirection, knockbackForce);
	}

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth; // Hồi đầy máu sau khi tăng
        Debug.Log("Increased Max HP: " + maxHealth);
    }
    public void TakeDamage(int damage)
	{
		if (isDead) return;

		currentHealth -= damage;
		currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
		UpdateHealthBar();

		if (playerAnimator != null && currentHealth > 0 && damage > 20)
		{
			playerAnimator.SetTrigger("IsHurt");
		}

		if (currentHealth <= 0 && !isDead)
		{
			StartCoroutine(HandleDeath());
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
		isDead = true;

		if (playerController != null)
		{
			playerController.TriggerDeathAnimation();
			playerController.enabled = false;
			playerAttack.enabled = false;
		}

		yield return new WaitForSeconds(2.7f);

		Respawn();
	}

	private void Respawn()
	{
		if (playerController != null)
		{
			playerController.enabled = true;
			playerAttack.enabled = true;
			playerController.ResetAfterRespawn();
		}

		currentHealth = maxHealth;
		isDead = false;
		UpdateHealthBar();
	}
}
