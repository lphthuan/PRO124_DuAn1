using UnityEngine;
using System.Collections;

public class ShadowController : MonoBehaviour
{
    // CÁC CHỈ SỐ CƠ BẢN
    public int health = 20;
    public int attackDamage = 5;
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float detectionRange = 7f;
    public float attackCooldown = 2f;

    // CÁC BIẾN ĐIỀU KHIỂN AI
    private bool canAttack = true;
    private Transform playerTransform;
    private enum EnemyState { Idle, Chase, Attack };
    private EnemyState currentState;

    // CÁC THÀNH PHẦN THAM CHIẾU
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Attack Settings")]
    public Transform attackPoint; // Điểm tấn công, gán từ Unity Editor
    public float attackRadius = 0.5f; // Bán kính vùng tấn công
    public LayerMask playerLayer; // Layer của người chơi

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Tìm object người chơi bằng tag "Player"
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        // Bắt đầu ở trạng thái đứng yên
        currentState = EnemyState.Idle;
    }

    void Update()
    {
        if (health <= 0) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;
            case EnemyState.Chase:
                ChaseState();
                break;
            case EnemyState.Attack:
                AttackState();
                break;
        }
    }

    void SetRunningState(bool isRun)
    {
        animator.SetBool("IsRun", isRun);
    }

    // Đảm bảo Shadow luôn hướng mặt về người chơi
    void FlipToPlayer()
    {
        if (playerTransform == null) return;
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void IdleState()
    {
        rb.velocity = Vector2.zero; // Đảm bảo quái vật đứng yên
        SetRunningState(false);

        // Kiểm tra người chơi
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer < detectionRange)
            {
                currentState = EnemyState.Chase;
            }
        }
    }

    void ChaseState()
    {
        if (playerTransform == null)
        {
            currentState = EnemyState.Idle;
            return;
        }

        // Luôn hướng mặt về người chơi trong trạng thái đuổi theo
        FlipToPlayer();
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > detectionRange)
        {
            currentState = EnemyState.Idle;
            rb.velocity = Vector2.zero;
        }
        else if (distanceToPlayer <= attackRange && canAttack)
        {
            currentState = EnemyState.Attack;
            rb.velocity = Vector2.zero;
            StartCoroutine(PerformAttackAndCooldown());
        }
        else
        {
            SetRunningState(true);
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
    }

    void AttackState()
    {
        SetRunningState(false);
        rb.velocity = Vector2.zero;
    }

    IEnumerator PerformAttackAndCooldown()
    {
        canAttack = false;

        // Bắt đầu animation tấn công
        animator.SetBool("IsAttack", true);

        // Chờ một khoảng thời gian ngắn để khớp với animation
        yield return new WaitForSeconds(0.5f);

        // Gây sát thương cho người chơi
        MeleeAttack();

        // Tắt animation tấn công
        animator.SetBool("IsAttack", false);

        // Chờ hết thời gian hồi chiêu
        yield return new WaitForSeconds(attackCooldown);

        // Cho phép tấn công lại và quay lại trạng thái đuổi theo
        canAttack = true;
        currentState = EnemyState.Chase;
    }

    void MeleeAttack()
    {
        // Kiểm tra tất cả collider trong một hình tròn tại attackPoint
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);

        foreach (Collider2D playerCollider in hitPlayers)
        {
            // Tìm và gọi hàm TakeDamage trên script PlayerHealth
            PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        animator.SetTrigger("IsHit");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetBool("IsDeath", true);
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ Gizmo cho tầm phát hiện và tấn công
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ Gizmo cho điểm tấn công
        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}