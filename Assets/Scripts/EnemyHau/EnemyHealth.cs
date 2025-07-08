using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 3f;
    private Animator anim;
    private bool isDead = false;
    public float currentHealth { get; private set; }

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = health;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bullet"))
        {
            TakeDamage(1f);
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage(float amount) 
    {
        if (isDead) return;

        health -= amount;
        currentHealth = health;

        if (health > 0)
        {
            anim?.SetTrigger("IsHit");
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim?.SetTrigger("IsDead");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 1f);
    }
}
