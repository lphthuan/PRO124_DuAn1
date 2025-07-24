using UnityEngine;
using System.Collections;

public class EnemyMain : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 5;
    [SerializeField] private int currentHealth;

    [Header("Detection & Attack Settings")]
    public Transform player;
    public float detectionRange = 6f;
    public float attackRange = 2f;
    public float fireCooldown = 1f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject miniEnemyPrefab;

    [Header("Sound Effects")]
    public AudioClip hitClip;
    public AudioClip dieClip;
    public AudioClip spawnMiniClip;
    public AudioClip shootClip;

    private AudioSource audioSource;
    private int attackCount = 0;
    private Animator animator;
    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }
    }

    void Update()
    {
        
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            return; 
        }

        if (isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            Vector3 scale = transform.localScale;
            scale.x = (player.position.x < transform.position.x) ? -1 : 1;
            transform.localScale = scale;

            if (distance <= attackRange)
            {
                if (!isAttacking)
                {
                    StartCoroutine(AttackRoutine());
                }
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (attackCount >= 5)
        {
            animator.SetTrigger("SpawnMiniEnemy");
            PlaySound(spawnMiniClip);
            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < 2; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(i == 0 ? -1 : 1, 0.5f, 0);
                Instantiate(miniEnemyPrefab, spawnPos, Quaternion.identity);
            }

            attackCount = 0;
        }
        else
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.2f);

            if (bulletPrefab != null && firePoint != null)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                PlaySound(shootClip);
            }

            attackCount++;
        }

        yield return new WaitForSeconds(fireCooldown);
        isAttacking = false;
    }

    private void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hit");
        PlaySound(hitClip);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        PlaySound(dieClip);
        Destroy(gameObject, 1.2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
