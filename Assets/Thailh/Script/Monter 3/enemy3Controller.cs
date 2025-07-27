using UnityEngine;
using System.Collections;

public class enemyController : MonoBehaviour
{
    [Header("Điểm tuần tra")]
    public Transform pointA;
    public Transform pointB;

    [Header("Cấu hình di chuyển")]
    public float patrolSpeed = 3f;
    public float chaseSpeed = 5f;

    [Header("Tầm phát hiện và tấn công")]
    public float detectionRange = 5f;
    public float attackRange = 3f;
    public Transform player;

    [Header("Tấn công tầm xa")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Trạng thái chịu tác động")]
    public bool canMove = true;

    private enum State { Walk, Run, Attack }
    private State currentState = State.Walk;
    private Transform currentTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentTarget = pointB;

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
        if (player == null || pointA == null || pointB == null || firePoint == null || fireballPrefab == null)
        {
            Debug.LogWarning("Thiếu tham chiếu trong Inspector!");
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
            currentState = State.Attack;
        else if (distanceToPlayer < detectionRange)
            currentState = State.Run;
        else
            currentState = State.Walk;

        switch (currentState)
        {
            case State.Walk:
                Patrol();
                break;
            case State.Run:
                RunToPlayer();
                break;
            case State.Attack:
                AttackPlayer();
                break;
        }
    }

    void Patrol()
    {
        if (!canMove) return;

        animator.SetBool("IsWalk", true);
        animator.SetBool("IsRun", false);
        animator.SetBool("IsAttack", false);

        Vector2 dir = (currentTarget.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * patrolSpeed, rb.velocity.y);
        Flip(dir.x);

        if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.1f)
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
    }

    void RunToPlayer()
    {
        if (!canMove) return;

        animator.SetBool("IsWalk", false);
        animator.SetBool("IsRun", true);
        animator.SetBool("IsAttack", false);

        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * chaseSpeed, rb.velocity.y);
        Flip(dir.x);
    }

    void AttackPlayer()
    {
        if (!canMove) return;

        animator.SetBool("IsWalk", false);
        animator.SetBool("IsRun", false);
        animator.SetBool("IsAttack", true);

        rb.velocity = new Vector2(0, rb.velocity.y);
        Flip(player.position.x - transform.position.x);
    }

    public void ShootFireball()
    {
        if (player == null || fireballPrefab == null || firePoint == null) return;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            Vector2 direction = (player.position - firePoint.position).normalized;
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            fireball.GetComponent<Fireball>().SetDirection(direction);
        }
    }

    void Flip(float directionX)
    {
        if (directionX > 0)
        {
            spriteRenderer.flipX = false;
            if (firePoint != null)
                firePoint.localPosition = new Vector2(Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y);
        }
        else if (directionX < 0)
        {
            spriteRenderer.flipX = true;
            if (firePoint != null)
                firePoint.localPosition = new Vector2(-Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // gió khống chế
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WindSpell"))
        {
            if (!canMove) return;

            canMove = false;

            // Hướng hất ngược lại WindSpell
            Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;

            if (rb != null)
            {
                rb.velocity = Vector2.zero; // Ngắt chuyển động
                float knockbackForce = 5f;
                rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }

            StartCoroutine(RestoreMovement());
        }
    }

    IEnumerator RestoreMovement()
    {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
    }
}
