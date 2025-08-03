using UnityEngine;
using System.Collections;

public class ShadowController : MonoBehaviour
{
    // CÁC CHỈ SỐ CƠ BẢN
    public int health = 20;
    public int attackDamage = 5;
    public float moveSpeed = 3f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.3f;
    public float attackRange = 1.5f; // Khoảng cách để tấn công người chơi
    public float detectionRange = 7f; // Khoảng cách để phát hiện người chơi
    public float attackCooldown = 2f; // Thời gian hồi chiêu tấn công

    // CÁC BIẾN ĐIỀU KHIỂN AI
    private bool canAttack = true;
    private Transform playerTransform;
    private enum EnemyState { Idle, Patrol, Chase, Attack, Dash }; // Các trạng thái AI
    private EnemyState currentState;

    // CÁC THÀNH PHẦN THAM CHIẾU
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Tìm object người chơi
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        // Bắt đầu ở trạng thái tuần tra (Idle hoặc Patrol)
        currentState = EnemyState.Idle;
    }

    void Update()
    {
        // Chuyển đổi giữa các trạng thái AI
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

        // Cập nhật animation
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        // Cập nhật tham số tốc độ di chuyển cho Animator
        // Mathf.Abs(rb.velocity.x) sẽ trả về giá trị dương dù quái vật di chuyển trái hay phải
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void IdleState()
    {
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // Nếu người chơi ở trong phạm vi phát hiện, chuyển sang trạng thái Chase
            if (distanceToPlayer < detectionRange)
            {
                currentState = EnemyState.Chase;
            }
        }
        // Có thể thêm logic tuần tra ngẫu nhiên ở đây nếu muốn
    }

    void ChaseState()
    {
        // Dừng quái vật lại nếu không tìm thấy người chơi
        if (playerTransform == null)
        {
            currentState = EnemyState.Idle;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Nếu người chơi ra ngoài tầm phát hiện, quay về trạng thái Idle
        if (distanceToPlayer > detectionRange)
        {
            currentState = EnemyState.Idle;
            // Dừng di chuyển
            rb.velocity = Vector2.zero;
        }
        // Nếu người chơi ở trong tầm tấn công, chuyển sang trạng thái Attack
        else if (distanceToPlayer <= attackRange && canAttack)
        {
            currentState = EnemyState.Attack;
            rb.velocity = Vector2.zero;
        }
        // Ngược lại, truy đuổi người chơi
        else
        {
            // Tính hướng đến người chơi
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            // Di chuyển quái vật
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            // Lật sprite theo hướng di chuyển
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
        // Kích hoạt animation tấn công
        animator.SetBool("IsAttack", true);

        // Bắt đầu Coroutine để xử lý việc tấn công và hồi chiêu
        StartCoroutine(PerformAttackAndCooldown());

        // Chuyển về trạng thái Chase sau khi tấn công để tiếp tục đuổi theo
        currentState = EnemyState.Chase;
    }

    IEnumerator PerformAttackAndCooldown()
    {
        // Gán biến canAttack thành false để quái vật không tấn công liên tục
        canAttack = false;

        // Chờ một khoảng thời gian ngắn để animation tấn công diễn ra
        yield return new WaitForSeconds(0.5f); // Điều chỉnh thời gian này cho phù hợp với animation của bạn

        // Gây sát thương
        // Ở đây, bạn cần thêm logic kiểm tra va chạm để gây sát thương cho người chơi
        // Ví dụ:
        // Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        // foreach(Collider2D hit in hitPlayers) {
        //     if (hit.CompareTag("Player")) {
        //         hit.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        //     }
        // }

        // Tắt animation tấn công
        animator.SetBool("IsAttack", false);

        // Chờ hết thời gian hồi chiêu
        yield return new WaitForSeconds(attackCooldown);

        // Gán canAttack thành true để quái vật có thể tấn công lại
        canAttack = true;
    }

    // Các hàm khác như TakeDamage, Die... giữ nguyên như hướng dẫn trước
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
        rb.velocity = Vector2.zero; // Dừng di chuyển khi chết
    }
}