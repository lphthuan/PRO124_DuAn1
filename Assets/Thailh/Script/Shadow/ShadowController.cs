using UnityEngine;
using UnityEngine.Events;

public class ShadowController : MonoBehaviour
{
    // Các biến có thể chỉnh sửa trong Inspector
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;

    [Header("Stats")]
    public int maxHealth = 50;
    public int currentHealth;
    public int healThreshold = 25; // Ngưỡng máu để hồi phục

    [Header("Heal")]
    public float healCooldown = 30f; // Thời gian cooldown của heal
    private float lastHealTime;
    public float healDuration = 3f; // Thời gian heal
    private float healTimer;

    [Header("Attack 1")]
    public float attackRange = 3f;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
    public UnityEvent onAttackHit;

    [Header("Attack 2")]
    public float attack2Range = 1.5f;
    public float attack2Damage = 20f;
    public UnityEvent onAttack2Hit;

    // Các tham chiếu đến component
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    // Biến trạng thái
    private enum State { Idle, Patrol, Attack, Attack2, Heal, Hit, Death };
    private State currentState;
    private State previousState; // Lưu trạng thái trước đó để quay lại sau khi Hit
    private Transform targetPoint;
    private bool isFacingRight = true;
    private float lastAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Tìm Player có tag "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentState = State.Patrol;
        targetPoint = pointA;
        lastHealTime = -healCooldown; // Cho phép hồi máu ngay lần đầu tiên

        // Khởi tạo isFacingRight dựa trên hướng ban đầu của quái vật
        isFacingRight = transform.localScale.x > 0;
    }

    void Update()
    {
        // Chuyển đổi giữa các trạng thái
        switch (currentState)
        {
            case State.Patrol:
                PatrolState();
                break;
            case State.Attack:
                AttackState();
                break;
            case State.Attack2:
                Attack2State();
                break;
            case State.Heal:
                HealState();
                break;
            case State.Hit:
                // Trạng thái Hit không cần update, chỉ chờ animation kết thúc
                break;
            case State.Death:
                // Không làm gì khi quái vật chết
                break;
        }
    }

    // --- Phương thức nhận sát thương ---
    public void TakeDamage(int damage)
    {
        // Quái vật không nhận sát thương khi đang ở trạng thái Heal hoặc Death
        if (currentState == State.Heal || currentState == State.Death)
        {
            return;
        }

        currentHealth -= damage;

        // Kích hoạt trạng thái Hit nếu còn sống
        if (currentHealth > 0)
        {
            previousState = currentState; // Lưu trạng thái hiện tại
            currentState = State.Hit;
            animator.SetTrigger("IsHit");
            rb.velocity = Vector2.zero; // Dừng lại khi bị đánh
        }

        // Kiểm tra điều kiện chết
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            currentState = State.Death;
            animator.SetBool("IsDead", true);
            rb.velocity = Vector2.zero;
        }

        Debug.Log("Shadow có " + currentHealth + " máu.");
    }

    // --- Các phương thức xử lý trạng thái ---

    private void PatrolState()
    {
        // Kiểm tra điều kiện hồi máu trước tiên
        if (currentHealth < healThreshold && Time.time > lastHealTime + healCooldown)
        {
            currentState = State.Heal;
            animator.SetBool("IsHeal", true);
            rb.velocity = Vector2.zero; // Dừng di chuyển
            healTimer = 0f; // Reset bộ đếm thời gian heal
            return;
        }

        animator.SetBool("IsRun", true);

        // Di chuyển đến điểm đích
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        // Lấy tọa độ X của điểm đến tiếp theo
        float nextPointX = targetPoint.position.x;

        // Kiểm tra xem quái vật đã đi qua điểm đích chưa
        bool hasPassedPoint = (isFacingRight && transform.position.x >= nextPointX) ||
                              (!isFacingRight && transform.position.x <= nextPointX);

        // Nếu đã đến đích, đổi hướng và lật mặt
        if (hasPassedPoint)
        {
            // Đổi điểm đích
            if (targetPoint == pointA)
            {
                targetPoint = pointB;
            }
            else
            {
                targetPoint = pointA;
            }

            Flip();
        }

        // Kiểm tra phạm vi tấn công
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attack2Range)
        {
            currentState = State.Attack2;
            animator.SetBool("IsRun", false);
        }
        else if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attack;
            animator.SetBool("IsRun", false);
        }
    }

    private void AttackState()
    {
        // Kiểm tra điều kiện hồi máu
        if (currentHealth < healThreshold && Time.time > lastHealTime + healCooldown)
        {
            currentState = State.Heal;
            animator.SetBool("IsHeal", true);
            rb.velocity = Vector2.zero;
            healTimer = 0f;
            return;
        }

        // Dừng di chuyển và quay mặt về phía player
        rb.velocity = Vector2.zero;
        LookAtPlayer();

        // Kiểm tra cooldown và thực hiện tấn công
        if (Time.time > lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("AttackTrigger");
            lastAttackTime = Time.time;
        }

        // Kiểm tra lại phạm vi, nếu player ra xa thì trở về tuần tra
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = State.Patrol;
        }
        else if (distanceToPlayer <= attack2Range)
        {
            currentState = State.Attack2;
        }
    }

    private void Attack2State()
    {
        // Kiểm tra điều kiện hồi máu
        if (currentHealth < healThreshold && Time.time > lastHealTime + healCooldown)
        {
            currentState = State.Heal;
            animator.SetBool("IsHeal", true);
            rb.velocity = Vector2.zero;
            healTimer = 0f;
            return;
        }

        // Dừng di chuyển và quay mặt về phía player
        rb.velocity = Vector2.zero;
        LookAtPlayer();

        animator.SetTrigger("Attack2Trigger");

        // Kiểm tra lại phạm vi, nếu player ra xa thì chuyển về trạng thái Attack
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > attack2Range)
        {
            currentState = State.Attack;
        }
    }

    private void HealState()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("IsHeal", true);

        healTimer += Time.deltaTime;
        if (healTimer >= healDuration)
        {
            currentHealth = maxHealth;
            lastHealTime = Time.time; // Cập nhật thời gian hồi máu cuối cùng

            animator.SetBool("IsHeal", false);
            CheckPlayerRangeAndSetState();
        }
    }

    // --- Các phương thức hỗ trợ ---

    // Hàm lật quái vật đã được tối ưu
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    // Hàm quay mặt về phía player
    private void LookAtPlayer()
    {
        // Xác định hướng của player so với quái vật
        bool playerIsRight = player.position.x > transform.position.x;

        // Nếu hướng của quái vật không khớp với hướng của player, hãy lật mặt
        if (playerIsRight != isFacingRight)
        {
            Flip();
        }
    }

    // Hàm kiểm tra tầm và chuyển trạng thái
    private void CheckPlayerRangeAndSetState()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attack2Range)
        {
            currentState = State.Attack2;
        }
        else if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attack;
        }
        else
        {
            currentState = State.Patrol;
        }
    }

    // --- Các phương thức sự kiện (được gọi từ Animation Event) ---

    public void OnAttackEvent()
    {
        onAttackHit.Invoke();
    }

    public void OnAttack2Event()
    {
        onAttack2Hit.Invoke();
    }

    // Sự kiện khi animation Hit kết thúc
    public void OnHitAnimationEnd()
    {
        CheckPlayerRangeAndSetState();
    }
}