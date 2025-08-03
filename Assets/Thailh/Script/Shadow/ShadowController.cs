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

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > detectionRange)
        {
            currentState = EnemyState.Idle;
            rb.velocity = Vector2.zero;
        }
        else if (distanceToPlayer <= attackRange && canAttack)
        {
            // Chuyển trạng thái sang tấn công và bắt đầu Coroutine
            currentState = EnemyState.Attack;
            rb.velocity = Vector2.zero;
            StartCoroutine(PerformAttackAndCooldown());
        }
        else
        {
            SetRunningState(true);
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    void AttackState()
    {
        // Trạng thái này không cần làm gì nhiều, vì Coroutine đã xử lý
        // Đảm bảo quái vật không chạy khi đang tấn công
        SetRunningState(false);
        rb.velocity = Vector2.zero;
    }

    IEnumerator PerformAttackAndCooldown()
    {
        canAttack = false;

        // Bắt đầu animation tấn công
        animator.SetBool("IsAttack", true);

        // Chờ một khoảng thời gian ngắn để animation tấn công diễn ra
        yield return new WaitForSeconds(0.5f);

        // Tắt animation tấn công sau khi nó đã chạy được một phần
        animator.SetBool("IsAttack", false);

        // Gây sát thương ở đây
        // VD: playerTransform.GetComponent<PlayerHealth>().TakeDamage(attackDamage);

        // Chờ hết thời gian hồi chiêu
        yield return new WaitForSeconds(attackCooldown);

        // Cho phép tấn công lại và quay lại trạng thái đuổi theo
        canAttack = true;
        currentState = EnemyState.Chase;
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}