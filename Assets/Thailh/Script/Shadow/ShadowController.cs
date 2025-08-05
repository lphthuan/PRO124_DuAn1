using UnityEngine;
using System.Collections;

public class ShadowController : MonoBehaviour
{
    //// Các biến có thể chỉnh sửa trong Inspector
    //[Header("Patrol")]
    //public Transform pointA;
    //public Transform pointB;
    //public float patrolSpeed = 2f;

    //[Header("Stats")]
    //public int maxHealth = 50;
    //public int currentHealth;
    //public int healThreshold = 25;

    //[Header("Heal")]
    //public float healCooldown = 30f;
    //private float lastHealTime;
    //public float healDuration = 3f;
    //private float healTimer;

    //[Header("Attack 1")]
    //public float attackRange = 3f;
    //public float attackDamage = 10f;
    //public float attackCooldown = 2f;
    //public Transform attackPoint1;
    //public float attackRadius1 = 0.5f;

    //[Header("Attack 2")]
    //public float attack2Range = 1.5f;
    //public float attack2Damage = 20f;
    //public Transform attackPoint2;
    //public float attackRadius2 = 0.5f;

    //[Header("Physics Detection")]
    //public LayerMask playerLayer;

    //[Header("Defense")]
    //public float defendDuration = 2f;
    //private float defendTimer;

    //// Các tham chiếu đến component
    //private Rigidbody2D rb;
    //private Animator animator;
    //private Transform player;

    //// Biến trạng thái
    //private enum State { Idle, Patrol, Attack, Attack2, Heal, Hit, Defend, Death };
    //private State currentState;
    //private Transform targetPoint;
    //private bool isFacingRight = true;
    //private float lastAttackTime;
    //private bool hasTriggeredAttack = false;
    //private float hitTimer;

    //void Start()
    //{
    //    rb = GetComponent<Rigidbody2D>();
    //    animator = GetComponent<Animator>();
    //    currentHealth = maxHealth;

    //    GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
    //    if (playerObject != null)
    //    {
    //        player = playerObject.transform;
    //    }

    //    currentState = State.Patrol;
    //    targetPoint = pointA;
    //    lastHealTime = -healCooldown;
    //    isFacingRight = transform.localScale.x > 0;
    //}

    //void Update()
    //{
    //    // ⭐ Xử lý các trạng thái ưu tiên cao nhất trước
    //    if (currentState == State.Death)
    //    {
    //        return;
    //    }

    //    if (currentState == State.Heal)
    //    {
    //        HealState();
    //        return;
    //    }

    //    if (currentState == State.Defend)
    //    {
    //        DefendState();
    //        return;
    //    }

    //    if (currentState == State.Hit)
    //    {
    //        HandleHitState();
    //        return;
    //    }

    //    // ⭐ Kiểm tra điều kiện chuyển trạng thái hồi máu ngay lập tức
    //    if (currentHealth < healThreshold && Time.time > lastHealTime + healCooldown)
    //    {
    //        currentState = State.Heal;
    //        SetAnimatorBools(true, false, false, false, false, false);
    //        rb.velocity = Vector2.zero;
    //        healTimer = 0f;
    //        return; // Đặt return để không chạy các logic phía dưới
    //    }

    //    // Kiểm tra khoảng cách và chuyển trạng thái tấn công hoặc tuần tra
    //    if (player != null)
    //    {
    //        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

    //        if (distanceToPlayer <= attack2Range)
    //        {
    //            currentState = State.Attack2;
    //            SetAnimatorBools(false, false, false, true, false, false);
    //        }
    //        else if (distanceToPlayer <= attackRange)
    //        {
    //            currentState = State.Attack;
    //            SetAnimatorBools(false, false, true, false, false, false);
    //        }
    //        else
    //        {
    //            currentState = State.Patrol;
    //            SetAnimatorBools(false, false, false, false, false, true);
    //            LookAtTarget(targetPoint);
    //        }
    //    }
    //    else
    //    {
    //        if (currentState != State.Patrol)
    //        {
    //            currentState = State.Patrol;
    //            SetAnimatorBools(false, false, false, false, false, true);
    //            LookAtTarget(targetPoint);
    //        }
    //    }

    //    switch (currentState)
    //    {
    //        case State.Patrol:
    //            PatrolState();
    //            break;
    //        case State.Attack:
    //            AttackState();
    //            break;
    //        case State.Attack2:
    //            Attack2State();
    //            break;
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("WindSpell"))
    //    {
    //        Debug.Log("Shadow phát hiện WindSpell và đang phòng thủ!");
    //        currentState = State.Defend;
    //        SetAnimatorBools(false, false, false, false, true, false);
    //        rb.velocity = Vector2.zero;
    //        defendTimer = 0f;
    //        LookAtTarget(other.transform);
    //        return;
    //    }

    //    if (other.CompareTag("PlayerBullet"))
    //    {
    //        float damage = 0;
    //        if (PlayerAttack.Instance != null)
    //        {
    //            damage = PlayerAttack.Instance.GetDamage();
    //        }
    //        else
    //        {
    //            damage = 10;
    //        }
    //        TakeDamage(damage, other.gameObject);
    //        Destroy(other.gameObject);
    //    }

    //    if (currentState == State.Defend)
    //    {
    //        if (other.CompareTag("PlayerBullet") || other.CompareTag("WindSpell"))
    //        {
    //            Debug.Log("Shadow đã phòng thủ thành công một đòn tấn công!");
    //            Destroy(other.gameObject);
    //            return;
    //        }
    //    }
    //}

    //public void DealDamageAttack1()
    //{
    //    if (attackPoint1 == null) return;
    //    Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint1.position, attackRadius1, playerLayer);
    //    foreach (Collider2D hit in hitObjects)
    //    {
    //        PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
    //        if (playerHealth != null)
    //        {
    //            playerHealth.TakeDamage((int)attackDamage);
    //        }
    //    }
    //}

    //public void DealDamageAttack2()
    //{
    //    if (attackPoint2 == null) return;
    //    Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint2.position, attackRadius2, playerLayer);
    //    foreach (Collider2D hit in hitObjects)
    //    {
    //        PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
    //        if (playerHealth != null)
    //        {
    //            playerHealth.TakeDamage((int)attack2Damage);
    //        }
    //    }
    //}

    //public void TakeDamage(float damage, GameObject source)
    //{
    //    if (currentState == State.Heal || currentState == State.Defend || currentState == State.Death)
    //    {
    //        return;
    //    }

    //    currentHealth -= (int)damage;
    //    Debug.Log("Shadow có " + currentHealth + " máu.");

    //    if (currentHealth <= 0)
    //    {
    //        currentHealth = 0;
    //        currentState = State.Death;
    //        // ⭐ Sửa lỗi: Chỉ bật IsDead, không tắt tất cả các animation khác
    //        animator.SetBool("IsDead", true);
    //        rb.velocity = Vector2.zero;
    //        Destroy(gameObject, 2f);
    //    }
    //    else
    //    {
    //        currentState = State.Hit;
    //        SetAnimatorBools(false, true, false, false, false, false);
    //        rb.velocity = Vector2.zero;
    //        hitTimer = 0f;
    //    }
    //}

    //private void HandleHitState()
    //{
    //    hitTimer += Time.deltaTime;
    //    float hitAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;

    //    if (hitTimer >= hitAnimationLength)
    //    {
    //        animator.SetBool("IsHit", false);
    //        hasTriggeredAttack = false;

    //        if (player == null)
    //        {
    //            currentState = State.Patrol;
    //            SetAnimatorBools(false, false, false, false, false, true);
    //            return;
    //        }

    //        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
    //        if (distanceToPlayer <= attack2Range)
    //        {
    //            currentState = State.Attack2;
    //            SetAnimatorBools(false, false, false, true, false, false);
    //        }
    //        else if (distanceToPlayer <= attackRange)
    //        {
    //            currentState = State.Attack;
    //            SetAnimatorBools(false, false, true, false, false, false);
    //        }
    //        else
    //        {
    //            currentState = State.Patrol;
    //            SetAnimatorBools(false, false, false, false, false, true);
    //        }
    //    }
    //}

    //private void PatrolState()
    //{
    //    if (player == null)
    //    {
    //        animator.SetBool("IsRun", true);
    //        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);
    //        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
    //        {
    //            if (targetPoint == pointA)
    //            {
    //                targetPoint = pointB;
    //            }
    //            else
    //            {
    //                targetPoint = pointA;
    //            }
    //            Flip();
    //        }
    //        return;
    //    }

    //    animator.SetBool("IsRun", true);
    //    hasTriggeredAttack = false;

    //    transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);
    //    if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
    //    {
    //        if (targetPoint == pointA)
    //        {
    //            targetPoint = pointB;
    //        }
    //        else
    //        {
    //            targetPoint = pointA;
    //        }
    //        Flip();
    //    }
    //}

    //private void AttackState()
    //{
    //    if (player == null)
    //    {
    //        currentState = State.Patrol;
    //        animator.SetBool("IsRun", true);
    //        return;
    //    }

    //    rb.velocity = Vector2.zero;
    //    LookAtPlayer();

    //    if (!hasTriggeredAttack && Time.time > lastAttackTime + attackCooldown)
    //    {
    //        animator.SetBool("IsAttack1", true);
    //        lastAttackTime = Time.time;
    //        hasTriggeredAttack = true;
    //    }
    //}

    //private void Attack2State()
    //{
    //    if (player == null)
    //    {
    //        currentState = State.Patrol;
    //        animator.SetBool("IsRun", true);
    //        return;
    //    }

    //    rb.velocity = Vector2.zero;
    //    LookAtPlayer();

    //    if (!hasTriggeredAttack)
    //    {
    //        animator.SetBool("IsAttack2", true);
    //        hasTriggeredAttack = true;
    //    }
    //}

    //private void HealState()
    //{
    //    rb.velocity = Vector2.zero;
    //    animator.SetBool("IsHeal", true);
    //    hasTriggeredAttack = false;

    //    healTimer += Time.deltaTime;
    //    if (healTimer >= healDuration)
    //    {
    //        currentHealth = maxHealth;
    //        lastHealTime = Time.time;
    //        animator.SetBool("IsHeal", false);

    //        if (player != null)
    //        {
    //            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
    //            if (distanceToPlayer <= attack2Range)
    //            {
    //                currentState = State.Attack2;
    //                SetAnimatorBools(false, false, false, true, false, false);
    //            }
    //            else if (distanceToPlayer <= attackRange)
    //            {
    //                currentState = State.Attack;
    //                SetAnimatorBools(false, false, true, false, false, false);
    //            }
    //            else
    //            {
    //                currentState = State.Patrol;
    //                SetAnimatorBools(false, false, false, false, false, true);
    //            }
    //        }
    //        else
    //        {
    //            currentState = State.Patrol;
    //            SetAnimatorBools(false, false, false, false, false, true);
    //        }
    //    }
    //}

    //private void DefendState()
    //{
    //    rb.velocity = Vector2.zero;
    //    defendTimer += Time.deltaTime;

    //    if (defendTimer >= defendDuration)
    //    {
    //        animator.SetBool("IsDefend", false);

    //        if (player != null)
    //        {
    //            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
    //            if (distanceToPlayer <= attack2Range)
    //            {
    //                currentState = State.Attack2;
    //                SetAnimatorBools(false, false, false, true, false, false);
    //            }
    //            else if (distanceToPlayer <= attackRange)
    //            {
    //                currentState = State.Attack;
    //                SetAnimatorBools(false, false, true, false, false, false);
    //            }
    //            else
    //            {
    //                currentState = State.Patrol;
    //                SetAnimatorBools(false, false, false, false, false, true);
    //            }
    //        }
    //        else
    //        {
    //            currentState = State.Patrol;
    //            SetAnimatorBools(false, false, false, false, false, true);
    //        }
    //    }
    //}

    //private void Flip()
    //{
    //    isFacingRight = !isFacingRight;
    //    Vector3 scaler = transform.localScale;
    //    scaler.x *= -1;
    //    transform.localScale = scaler;
    //}

    //private void LookAtPlayer()
    //{
    //    if (player == null) return;
    //    bool playerIsRight = player.position.x > transform.position.x;
    //    if (playerIsRight != isFacingRight)
    //    {
    //        Flip();
    //    }
    //}

    //private void LookAtTarget(Transform target)
    //{
    //    if (target == null) return;
    //    bool targetIsRight = target.position.x > transform.position.x;
    //    if (targetIsRight != isFacingRight)
    //    {
    //        Flip();
    //    }
    //}

    //private void SetAnimatorBools(bool isHeal, bool isHit, bool isAttack1, bool isAttack2, bool isDefend, bool isRun)
    //{
    //    animator.SetBool("IsHeal", isHeal);
    //    animator.SetBool("IsHit", isHit);
    //    animator.SetBool("IsAttack1", isAttack1);
    //    animator.SetBool("IsAttack2", isAttack2);
    //    animator.SetBool("IsDefend", isDefend);
    //    animator.SetBool("IsRun", isRun);
    //}

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, attackRange);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, attack2Range);

    //    if (attackPoint1 != null)
    //    {
    //        Gizmos.color = Color.blue;
    //        Gizmos.DrawWireSphere(attackPoint1.position, attackRadius1);
    //    }
    //    if (attackPoint2 != null)
    //    {
    //        Gizmos.color = Color.cyan;
    //        Gizmos.DrawWireSphere(attackPoint2.position, attackRadius2);
    //    }
    //}
}