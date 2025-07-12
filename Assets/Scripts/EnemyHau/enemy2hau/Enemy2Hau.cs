using System.Collections;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Shooting,
    Hit,
    DyingFirst,
    Respawning,
    Chasing,
    Attacking,
    DyingFinal
}

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 5;
    [SerializeField] private int currentHealth;

    [Header("Player Detection")]
    public Transform player;
    public float detectionRange = 6f;
    public float attackRange = 1.2f;
    public float fireCooldown = 1f;

    [Header("Combat - Ranged")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Combat - Melee")]
    public GameObject meleeHitboxPrefab;
    public Transform meleeSpawnPoint;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip hitClip;
    public AudioClip dieClip;
    public AudioClip teleportClip;
    public AudioClip meleeAttackClip;

    private bool hasDiedOnce = false;
    private EnemyState currentState = EnemyState.Idle;

    private Animator animator;
    private Coroutine attackShootRoutine;
    private Coroutine meleeAttackRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (currentState is EnemyState.DyingFirst or EnemyState.DyingFinal or EnemyState.Respawning or EnemyState.Hit)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                animator.SetBool("Walk", false);
                if (distance <= detectionRange && !hasDiedOnce)
                {
                    currentState = EnemyState.Shooting;
                }
                else if (hasDiedOnce && distance <= detectionRange)
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Shooting:
                animator.SetBool("Walk", false);
                FacePlayer();

                if (distance > detectionRange + 1f)
                {
                    currentState = EnemyState.Idle;
                    StopAttackRoutine();
                }
                else if (attackShootRoutine == null)
                {
                    attackShootRoutine = StartCoroutine(ChargeAndShoot());
                }
                break;

            case EnemyState.Chasing:
                animator.SetBool("Walk", true);
                FacePlayer();

                if (distance <= attackRange)
                {
                    currentState = EnemyState.Attacking;
                    animator.SetBool("Walk", false);
                }
                else if (distance <= detectionRange)
                {
                    ChasePlayer();
                }
                else
                {
                    currentState = EnemyState.Idle;
                    animator.SetBool("Walk", false);
                }
                break;

            case EnemyState.Attacking:
                FacePlayer();

                if (meleeAttackRoutine == null)
                {
                    meleeAttackRoutine = StartCoroutine(PerformMeleeAttack());
                }

                break;
        }
    }

    IEnumerator ChargeAndShoot()
    {
        animator.SetTrigger("AttackShoot");
        yield return new WaitForSeconds(2.5f);
        if (currentState == EnemyState.Shooting)
        {
            Shoot();
        }
        attackShootRoutine = null;
    }

    IEnumerator PerformMeleeAttack()
    {
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(1.5f);

        if (currentState == EnemyState.Attacking)
        {
            SpawnMeleeHitbox();
            PlaySound(meleeAttackClip);
        }

        yield return new WaitForSeconds(fireCooldown);

        float dist = Vector2.Distance(transform.position, player.position);
        currentState = (dist <= attackRange) ? EnemyState.Attacking : EnemyState.Chasing;

        meleeAttackRoutine = null;
    }

    void SpawnMeleeHitbox()
    {
        if (meleeHitboxPrefab != null && meleeSpawnPoint != null)
        {
            GameObject hitbox = Instantiate(meleeHitboxPrefab, meleeSpawnPoint.position, meleeSpawnPoint.rotation);
            Destroy(hitbox, 0.5f);
        }
    }

    void StopAttackRoutine()
    {
        if (attackShootRoutine != null)
        {
            StopCoroutine(attackShootRoutine);
            attackShootRoutine = null;
        }
    }

    void Shoot()
    {
        if (bulletPrefab && firePoint)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            PlaySound(shootClip);
        }
    }

    void FacePlayer()
    {
        if (!player) return;

        Vector3 dir = player.position - transform.position;
        transform.localScale = new Vector3(dir.x > 0 ? 1 : -1, 1, 1);
    }

    void ChasePlayer()
    {
        if (player)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentState == EnemyState.DyingFirst || currentState == EnemyState.DyingFinal) return;

        currentHealth -= amount;
        animator.SetTrigger("Hit");
        PlaySound(hitClip);

        if (currentHealth <= 0)
        {
            if (!hasDiedOnce)
            {
                currentState = EnemyState.DyingFirst;
                hasDiedOnce = true;
                animator.SetTrigger("Die");
                PlaySound(dieClip);
                StopAttackRoutine();
                StartCoroutine(RespawnRoutine());
            }
            else
            {
                currentState = EnemyState.DyingFinal;
                animator.SetTrigger("Die");
                PlaySound(dieClip);
                StopAttackRoutine();
                Destroy(gameObject, 2f);
            }
        }
        else
        {
            StartCoroutine(HitRecovery());
        }
    }

    IEnumerator HitRecovery()
    {
        currentState = EnemyState.Hit;
        animator.SetBool("Walk", false);
        yield return new WaitForSeconds(0.5f);
        currentState = hasDiedOnce ? EnemyState.Chasing : EnemyState.Shooting;
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        currentState = EnemyState.Respawning;
        animator.SetTrigger("TeleportSpawn");
        PlaySound(teleportClip);

        yield return new WaitForSeconds(1f);
        currentHealth = maxHealth;

        yield return new WaitForSeconds(3f);
        currentState = EnemyState.Idle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
