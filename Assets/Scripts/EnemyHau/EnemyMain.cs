using UnityEngine;

public class EnemyMain : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Attack Settings")]
    public Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject miniEnemyPrefab;

    public float fireCooldown = 1f;
    private float fireTimer = 0f;
    private int attackCount = 0;

    private bool playerInRange = false;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || player == null || !playerInRange) return;

       
        Vector3 scale = transform.localScale;
        scale.x = (player.position.x < transform.position.x) ? -1 : 1;
        transform.localScale = scale;

       
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireCooldown && attackCount < 5)
        {
            fireTimer = 0f;
            Fire();
            attackCount++;

            if (attackCount == 5)
            {
                SpawnMiniEnemies();
            }
        }
    }

    private void Fire()
    {
        animator.SetTrigger("Attack");

        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    private void SpawnMiniEnemies()
    {
        animator.SetTrigger("SpawnMiniEnemy");

        for (int i = 0; i < 2; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(i == 0 ? -1 : 1, 0.5f, 0);
            Instantiate(miniEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    private void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        Destroy(gameObject, 1.2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
        else if (other.CompareTag("PlayerBullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
