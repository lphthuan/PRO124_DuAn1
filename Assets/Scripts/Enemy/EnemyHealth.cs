using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public float currentHealth;
    private Animator anim;
    private bool isDead = false;
    [SerializeField] private Rigidbody2D enemyRigidbody;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = health;
    }

    public void TakeDamage(float damage, GameObject source)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        { 
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("WindSpell") || collision.CompareTag("PlayerBullet"))
        {
            float damage = PlayerAttack.Instance.GetDamage();
            TakeDamage(damage, collision.gameObject);

            Destroy(collision.gameObject);
        }
    }


    void Die()
    {
        isDead = true;
        anim?.SetTrigger("IsDead");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        enemyRigidbody.velocity = Vector2.zero;
        enemyRigidbody.isKinematic = true;


		Destroy(gameObject, 1f);
    }
}
