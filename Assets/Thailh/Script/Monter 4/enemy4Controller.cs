using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemy4Controller : MonoBehaviour
{
    [Header("Tuần tra")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 3f;

    [Header("Truy đuổi và tấn công")]
    public float chaseSpeed = 5f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int damage = 3;

    [Header("Tấn công phép")]
    public float fireAttackRange = 3f;
    public GameObject fireballPrefab;
    public float fireballSpeed = 7f;
    public float fireAttackCooldown = 5f;

    [Header("Máu")]
    public int maxHealth = 20;

    public Transform player;

    [Header("Cận chiến")]
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask playerLayer;

    [Header("Trạng thái đặc biệt")]
    public bool canMove = true;

    private Vector3 patrolTarget;
    private Animator animator;
    private float attackTimer;
    private float fireAttackTimer;
    private int currentHealth;
    private bool isDead = false;
    private bool isChasing = false;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        patrolTarget = pointB.position;
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        fireAttackTimer = 0f;

        if (player == null)
            Debug.LogWarning("Bạn chưa gán Transform Player cho enemy4!");

        if (attackPoint == null)
            Debug.LogWarning("Bạn chưa gán AttackPoint cho enemy4!");

        if (rb == null)
            Debug.LogWarning("Bạn cần thêm Rigidbody2D vào enemy4!");

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
        if (isDead || player == null || !canMove) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            isChasing = true;
            AttackPlayer();
        }
        else if (distanceToPlayer <= fireAttackRange)
        {
            isChasing = true;
            FireAttack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            if (isChasing)
            {
                animator.ResetTrigger("IsAttack");
                animator.SetBool("IsRun", true);

                patrolTarget = (transform.position.x < (pointA.position.x + pointB.position.x) / 2f)
                    ? pointB.position
                    : pointA.position;

                isChasing = false;
            }

            Patrol();
        }
    }

    void Patrol()
    {
        if (!canMove) return;

        animator.SetBool("IsRun", true);
        MoveTo(patrolTarget, patrolSpeed);

        if (Mathf.Abs(transform.position.x - patrolTarget.x) < 0.5f)
        {
            patrolTarget = (patrolTarget == pointA.position) ? pointB.position : pointA.position;
        }
    }

    void ChasePlayer()
    {
        if (!canMove) return;

        animator.SetBool("IsRun", true);
        MoveTo(player.position, chaseSpeed);
    }

    void MoveTo(Vector3 target, float speed)
    {
        if (!canMove) return;

        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (target.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    void AttackPlayer()
    {
        animator.SetBool("IsRun", false);

        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            animator.SetTrigger("IsAttack");

            Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);
            if (hit != null)
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }

            attackTimer = attackCooldown;
        }
    }

    void FireAttack()
    {
        animator.SetBool("IsRun", false);

        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        fireAttackTimer -= Time.deltaTime;
        if (fireAttackTimer <= 0f)
        {
            StartCoroutine(SummonFireballs());
            fireAttackTimer = fireAttackCooldown;
        }
    }

    IEnumerator SummonFireballs()
    {
        int fireballCount = 3;
        float spacing = 1.5f;
        Vector3 center = transform.position + new Vector3(0, 3f, 0);
        List<GameObject> fireballs = new List<GameObject>();

        for (int i = 0; i < fireballCount; i++)
        {
            float offsetX = (i - 1) * spacing;
            Vector3 spawnPos = center + new Vector3(offsetX, 0, 0);

            GameObject fireball = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
            fireballs.Add(fireball);
        }

        // Đợi x giây trước khi bắn
        yield return new WaitForSeconds(1f);

        // Bắn từng quả về phía Player
        foreach (GameObject fireball in fireballs)
        {
            if (fireball != null)
            {
                Vector2 direction = (player.position - fireball.transform.position).normalized;
                fireball.GetComponent<Rigidbody2D>().velocity = direction * fireballSpeed;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("IsDeath");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WindSpell"))
        {
            if (!canMove) return;

            canMove = false;
            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;

            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                float knockbackForce = 3f;
                rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }

            StartCoroutine(RestoreMovement());
        }
    }

    private IEnumerator RestoreMovement()
    {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, fireAttackRange);
    }
}
