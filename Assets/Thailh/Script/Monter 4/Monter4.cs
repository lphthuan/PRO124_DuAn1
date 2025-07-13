using UnityEngine;

public class Monter4 : MonoBehaviour
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

    [Header("Máu")]
    public int maxHealth = 20;

    [Header("Player")]
    public Transform player; // Kéo vào Inspector

    [Header("Cận chiến")]
    public Transform attackPoint;              // Điểm gốc để xác định vùng tấn công
    public float attackRadius = 1f;            // Bán kính vùng phun lửa
    public LayerMask playerLayer;              // Layer của Player để xác định mục tiêu

    private Vector3 patrolTarget;
    private Animator animator;
    private float attackTimer;
    private int currentHealth;
    private bool isDead = false;
    private bool isChasing = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        patrolTarget = pointB.position;
        currentHealth = maxHealth;

        if (player == null)
            Debug.LogWarning("Bạn chưa gán Transform Player cho Monter4!");

        if (attackPoint == null)
            Debug.LogWarning("Bạn chưa gán AttackPoint cho Monter4!");
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            isChasing = true;
            AttackPlayer();
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
        animator.SetBool("IsRun", true);
        MoveTo(patrolTarget, patrolSpeed);

        if (Mathf.Abs(transform.position.x - patrolTarget.x) < 0.05f)
        {
            patrolTarget = (patrolTarget == pointA.position) ? pointB.position : pointA.position;
        }
    }

    void ChasePlayer()
    {
        animator.SetBool("IsRun", true);
        MoveTo(player.position, chaseSpeed);
    }

    void MoveTo(Vector3 target, float speed)
    {
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

            // Kiểm tra nếu Player nằm trong vùng phun lửa
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
