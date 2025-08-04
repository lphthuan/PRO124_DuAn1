using UnityEngine;

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
    public int healThreshold = 25;

    [Header("Heal")]
    public float healCooldown = 30f;
    private float lastHealTime;
    public float healDuration = 3f;
    private float healTimer;

    [Header("Attack 1")]
    public float attackRange = 3f;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
    public Transform attackPoint1; // Điểm tấn công vật lý 1
    public float attackRadius1 = 0.5f;

    [Header("Attack 2")]
    public float attack2Range = 1.5f;
    public float attack2Damage = 20f;
    // Đã loại bỏ public float knockbackForce = 5f; 
    // Đã loại bỏ public float stunDuration = 1f;
    public Transform attackPoint2; // Điểm tấn công vật lý 2
    public float attackRadius2 = 0.5f;

    [Header("Physics Detection")]
    public LayerMask playerLayer; // Lớp của người chơi để kiểm tra va chạm

    [Header("Defense")]
    public float defendDuration = 2f; // Thời gian phòng thủ
    private float defendTimer;

    // Các tham chiếu đến component
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    // Biến trạng thái
    // Đã thêm trạng thái "Defend"
    private enum State { Idle, Patrol, Attack, Attack2, Heal, Hit, Defend, Death };
    private State currentState;
    private State previousState;
    private Transform targetPoint;
    private bool isFacingRight = true;
    private float lastAttackTime;
    private bool hasTriggeredAttack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Chỉ tìm Player để biết vị trí, không cần lấy component
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        currentState = State.Patrol;
        targetPoint = pointA;
        lastHealTime = -healCooldown;
        isFacingRight = transform.localScale.x > 0;
    }

    void Update()
    {
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
            case State.Defend:
                DefendState();
                break;
            case State.Hit:
                break;
            case State.Death:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu Shadow đang phòng thủ và va chạm với "WindSpell"
        if (currentState == State.Defend && other.CompareTag("WindSpell"))
        {
            Debug.Log("Shadow đã phòng thủ thành công một WindSpell!");
            // Hủy WindSpell mà không gây sát thương
            Destroy(other.gameObject);
            return; // Dừng xử lý va chạm để không gây sát thương
        }

        // Kiểm tra xem đối tượng va chạm có tag "PlayerBullet" không
<<<<<<< Updated upstream
        //if (other.CompareTag("PlayerBullet"))
        //{
            
        //    PlayerBullet bullet = other.GetComponent<PlayerBullet>();
        //    if (bullet != null)
        //    {
        //        // Gọi hàm TakeDamage để Shadow nhận sát thương từ viên đạn
        //        TakeDamage(bullet.damage);
        //    }

        //    // Hủy viên đạn sau khi va chạm
        //    Destroy(other.gameObject);
        //}
=======

        if (other.CompareTag("PlayerBullet"))
        {
            PlayerLightningSpell bullet = other.GetComponent<PlayerLightningSpell>();
            if (bullet != null)
            {
                // Gọi hàm TakeDamage để Shadow nhận sát thương từ viên đạn
                TakeDamage(bullet.damage);
            }

            // Hủy viên đạn sau khi va chạm
            Destroy(other.gameObject);
        }

        if (other.CompareTag("PlayerBullet"))
        {
            // Lấy component PlayerBullet để lấy giá trị sát thương
            PlayerLightningSpell bullet = other.GetComponent<PlayerLightningSpell>();
            if (bullet != null)
            {
                // Gọi hàm TakeDamage để Shadow nhận sát thương từ viên đạn
                TakeDamage(bullet.damage);
            }

            // Hủy viên đạn sau khi va chạm
            Destroy(other.gameObject);
        }

>>>>>>> Stashed changes
    }

    public void TakeDamage(int damage)
    {
        // Không nhận sát thương khi đang hồi máu hoặc đã chết
        if (currentState == State.Heal || currentState == State.Defend || currentState == State.Death)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth > 0)
        {
            previousState = currentState;
            currentState = State.Hit;
            animator.SetTrigger("IsHit");
            rb.velocity = Vector2.zero;
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            currentState = State.Death;
            animator.SetBool("IsDead", true);
            rb.velocity = Vector2.zero;
        }

        Debug.Log("Shadow có " + currentHealth + " máu.");
    }

    private void PatrolState()
    {
        if (currentHealth < healThreshold && Time.time > lastHealTime + healCooldown)
        {
            currentState = State.Heal;
            animator.SetBool("IsHeal", true);
            rb.velocity = Vector2.zero;
            healTimer = 0f;
            return;
        }

        animator.SetBool("IsRun", true);
        hasTriggeredAttack = false;

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);
        float nextPointX = targetPoint.position.x;
        bool hasPassedPoint = (isFacingRight && transform.position.x >= nextPointX) ||
                              (!isFacingRight && transform.position.x <= nextPointX);

        if (hasPassedPoint)
        {
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

        // Logic chuyển sang trạng thái Defend
        // Có thể thêm điều kiện khác để chuyển sang trạng thái phòng thủ
        // Ví dụ: khi Player chuẩn bị dùng đòn đánh mạnh
        // For demonstration, we will just add a timer-based check
        if (Time.time > lastAttackTime + attackCooldown)
        {
            if (Random.Range(0, 100) < 10) // 10% cơ hội để phòng thủ
            {
                currentState = State.Defend;
                animator.SetBool("IsDefend", true);
                rb.velocity = Vector2.zero;
                defendTimer = 0f;
            }
        }
    }

    private void AttackState()
    {
        if (currentHealth < healThreshold && Time.time > lastHealTime + healCooldown)
        {
            currentState = State.Heal;
            animator.SetBool("IsHeal", true);
            rb.velocity = Vector2.zero;
            healTimer = 0f;
            return;
        }

        rb.velocity = Vector2.zero;
        LookAtPlayer();

        if (!hasTriggeredAttack && Time.time > lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("AttackTrigger1");
            lastAttackTime = Time.time;
            hasTriggeredAttack = true;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = State.Patrol;
            hasTriggeredAttack = false;
        }
        else if (distanceToPlayer <= attack2Range)
        {
            currentState = State.Attack2;
            hasTriggeredAttack = false;
        }
    }

    private void Attack2State()
    {
        if (currentHealth < healThreshold && Time.time > lastHealTime + healCooldown)
        {
            currentState = State.Heal;
            animator.SetBool("IsHeal", true);
            rb.velocity = Vector2.zero;
            healTimer = 0f;
            return;
        }

        rb.velocity = Vector2.zero;
        LookAtPlayer();

        if (!hasTriggeredAttack)
        {
            // Kích hoạt animation tấn công 2
            animator.SetTrigger("AttackTrigger2");
            hasTriggeredAttack = true;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > attack2Range)
        {
            currentState = State.Attack;
            hasTriggeredAttack = false;
        }
    }

    private void HealState()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("IsHeal", true);
        hasTriggeredAttack = false;

        healTimer += Time.deltaTime;
        if (healTimer >= healDuration)
        {
            currentHealth = maxHealth;
            lastHealTime = Time.time;
            animator.SetBool("IsHeal", false);
            CheckPlayerRangeAndSetState();
        }
    }

    // === Hàm mới cho trạng thái phòng thủ ===
    private void DefendState()
    {
        rb.velocity = Vector2.zero;
        defendTimer += Time.deltaTime;

        if (defendTimer >= defendDuration)
        {
            animator.SetBool("IsDefend", false);
            CheckPlayerRangeAndSetState(); // Quay lại trạng thái hành động bình thường
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void LookAtPlayer()
    {
        if (player == null) return;
        bool playerIsRight = player.position.x > transform.position.x;
        if (playerIsRight != isFacingRight)
        {
            Flip();
        }
    }

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
        hasTriggeredAttack = false;
    }

    //được gọi từ Animation Event
    public void DealDamageAttack1()
    {
        if (attackPoint1 == null) return;

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint1.position, attackRadius1, playerLayer);
        foreach (Collider2D hit in hitObjects)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)attackDamage);
            }
        }
    }

    public void DealDamageAttack2()
    {
        if (attackPoint2 == null) return;

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint2.position, attackRadius2, playerLayer);
        foreach (Collider2D hit in hitObjects)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Chỉ gây st
                playerHealth.TakeDamage((int)attack2Damage);
            }
        }
    }

    public void OnHitAnimationEnd()
    {
        hasTriggeredAttack = false;
        CheckPlayerRangeAndSetState();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attack2Range);

        if (attackPoint1 != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint1.position, attackRadius1);
        }
        if (attackPoint2 != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPoint2.position, attackRadius2);
        }
    }
}