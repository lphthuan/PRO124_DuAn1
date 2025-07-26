using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float health = 100f;
    private Animator anim;
    private bool isDead = false;
    [SerializeField] private Rigidbody2D enemyRigidbody;
    public float currentHealth { get; private set; }

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = health;
    }

    public void TakeDamage(float damage, GameObject source)
    {
        if (isDead) return;

        health -= damage;

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
        enemyRigidbody.velocity = Vector2.zero;
        enemyRigidbody.isKinematic = true;


		Destroy(gameObject, 1f);
    }
}
