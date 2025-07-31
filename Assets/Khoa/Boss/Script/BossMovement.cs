    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private GameObject dmgArea;
    [SerializeField] GameObject MagicAreaCheck;
    [SerializeField] GameObject Magic;
    [SerializeField] GameObject SummonMagic;
    [SerializeField] GameObject SummonArenaFinalBoss;
    [HideInInspector] public Transform playerTransform;
    [SerializeField] public Transform BossTransform;
    [SerializeField] GameObject TeleZone;
    public Transform Player;
    public float speed = 2.5f;
    public float PlayerCheckRange = 10f;
    private bool isChasing = false;
    public Animator animator;
    public Transform visualTransform;
    private bool lastIsRunState = false;
    private bool isAttacking = false; //Boss đang Attack
    private bool isWaiting = false;   //Boss đang cooldown


    public bool castSkil = false;

    private MiniBossHealth bossHealth;
    private void Start()
    {
        if (Player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                Player = foundPlayer.transform;
            }
        }
        MagicAreaCheck.SetActive(false);
        bossHealth = GetComponent<MiniBossHealth>();
        bossHealth.OnBossDead += HandleBossDeath;
    }

    void Update()
    {
        Phase2Boss();
        if (!castSkil)
        {
            AttackMagicCoroutine();
        }
        if (isWaiting || isAttacking)
        {
            animator.SetBool("IsRun", false); //Đứng yên trong khi tấn công/cooldown
            return;
        }
        CountAttack();
        DetectPlayer();
        animatorController();

        if (isChasing)
        {
            MoveToPlayer();
        }
    }

    private void DetectPlayer()
    {
        if (isAttacking)
            return; // Nếu đang tấn công, không kiểm tra Player

        // Tìm tất cả Collider2D trong bán kính PlayerCheckRange
        Collider2D hit = Physics2D.OverlapCircle(transform.position, PlayerCheckRange, LayerMask.GetMask("Player"));

        if (hit != null && hit.CompareTag("Player"))
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }
    }


    private void animatorController()
    {
        if (isChasing != lastIsRunState)
        {
            animator.SetBool("IsRun", isChasing);
            lastIsRunState = isChasing;
        }
    }

    private void MoveToPlayer()
    {
        Vector3 targetPosition = new Vector3(Player.position.x, transform.position.y, transform.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        // Flip Boss
        if (direction.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }









    private int normalAttackCount = 0;
    private int MagicAttackCount = 0;
    public void OnHitPlayer(GameObject playerObj)
    {
        if (!isAttacking && normalAtkCount < 2) 
        {
            normalAttackCount++;
            StartCoroutine(AttackCoroutine());
        }
        if (normalAttackCount == 3)
        {
            castSkil = true;
        }
    }

    public void CountAttack()
    {
        if (normalAttackCount > 3 && MagicAttackCount < 3)
        {
            StartCoroutine(AttackMagicCoroutine());
            normalAttackCount = 0;
            MagicAttackCount++;
        }

    }


    public int phase = 1;
    public void Phase2Boss()
    {
        if (phase == 2)
        {
            return;
        }
        if (bossHealth.currentHealth <= 25)
        {
            StartCoroutine(SpawnMagicCoroutine());
            phase = 2;
        }
    }














    public void SpawnAttackCollider()
    {
        SpawnColliderMagicCheck();
        float huongQuai = (visualTransform != null) ? Mathf.Sign(visualTransform.localScale.x) : Mathf.Sign(transform.localScale.x);

        Vector3 offset = new Vector3(5 * huongQuai, 1, 0);
        GameObject vungSatThuong = Instantiate(dmgArea, transform.position + offset, Quaternion.identity);

        if (huongQuai < 0)
        {
            vungSatThuong.transform.localScale = new Vector3(-1, 1, 1);
        }

        Destroy(vungSatThuong, 0.3f);
    }

    public int deadCheck = 0;
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (deadCheck > 0)
            return;

        // Nếu còn khiên
        if (bossHealth.currentShield > 0)
        {
            if (collision.CompareTag("WindSpell"))
            {
                bossHealth.TakeShield(50f);
                return;
            }
            // Các loại khác không gây damage khi còn khiên
            return;
        }

        // Khi shield đã hết, mọi đòn tấn công đều gây damage
        if (collision.CompareTag("WindSpell") || collision.CompareTag("PlayerBullet"))
        {
            float damage = PlayerAttack.Instance.GetDamage(); // hoặc 1 giá trị cố định nếu không boost được
            bossHealth.TakeDamage(damage, collision.gameObject); // truyền cả damage và source

            if (bossHealth.currentHealth <= 0)
            {
                StartCoroutine(isDead());
                deadCheck++;
            }
        }

    }*/


    private int normalAtkCount = 0;
    private void SpawnColliderMagicCheck()
    {

        if (isAttacking)
        {
            normalAtkCount++;
            if (normalAtkCount == 2)
            {
                
                StartCoroutine(SpawnRadaCheckMagic());
                return; // Đảm bảo chỉ chạy radar, không làm gì thêm!
            }
        }
    }


    public void SpawnSkill1Effect()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform != null)
        {
            // Lấy vị trí player, cộng offset lên trên đầu
            Vector3 spawnPos = playerTransform.position + new Vector3(0, 2f, 0);

            GameObject vungtrieuhoi = Instantiate(Magic, spawnPos, Quaternion.identity); // cần học chỗ này
            Debug.Log("Skill1 được kích hoạt!");
            Destroy(vungtrieuhoi, 1.6f);
        }
    }
    public void SpawnModEffect1()
    {

            Vector3 spawnMod1 = BossTransform.position + new Vector3(2f, 1f, 0);
            GameObject SummonArea1 = Instantiate(SummonMagic, spawnMod1, Quaternion.identity);
            Debug.Log("Skill1 được kích hoạt!");
            Destroy(SummonArea1, 1.6f);


    }
    public void SpawnModEffect2()
    {
        Vector3 spawnMod2 = BossTransform.position + new Vector3(-2f, 1f, 0);
        GameObject SummonArea2 = Instantiate(SummonMagic, spawnMod2, Quaternion.identity);// cần học chỗ này
        Debug.Log("Skill1 được kích hoạt!");
        Destroy(SummonArea2, 1.6f);

    }

    public void SummonFinalBoss()
    {
        Vector3 spawnPos = new Vector3(0, 0, 0);
        GameObject ArenaFinalBoss = Instantiate(SummonArenaFinalBoss, spawnPos, Quaternion.identity);
        Debug.Log("Final Boss được triệu hồi!");
        Destroy(ArenaFinalBoss, 4f);
    }

    private void HandleBossDeath()
    {
        if (deadCheck > 0) return;

        StartCoroutine(isDead());
        deadCheck++;
    }



    //dưới đây để kiểm soát Coroutine




    private IEnumerator AttackCoroutine()
    {
        isChasing = false;
        isAttacking = true;
        isWaiting = true;

        animator.SetBool("IsRun", false);
        animator.SetBool("IsAtk", true);

        yield return new WaitForSeconds(0.8f);

        animator.SetBool("IsAtk", false);   
        isAttacking = false;                

        yield return new WaitForSeconds(1f);
        isWaiting = false;

        isChasing = true; 
        animator.SetBool("IsRun", true); 
        lastIsRunState = true;           // Cập nhật trạng thái
    }


    private IEnumerator AttackMagicCoroutine()
    {
        isChasing = false;
        isAttacking = true;
        isWaiting = true;

        animator.SetBool("IsRun", false);
        animator.SetBool("IsAtk2", true);

        yield return new WaitForSeconds(2.4f);

        animator.SetBool("IsAtk2", false);
        isAttacking = false;

        yield return new WaitForSeconds(2f);
        isWaiting = false;

        isChasing = true; 
        castSkil = false;

        animator.SetBool("IsRun", true); 
        lastIsRunState = true;           //Cập nhật trạng thái
        
    }
    private IEnumerator SpawnMagicCoroutine()
    {
        isChasing = false;
        isAttacking = true;
        isWaiting = true;

        animator.SetBool("IsRun", false);
        animator.SetBool("IsSpawn", true);

        yield return new WaitForSeconds(2.4f);

        animator.SetBool("IsSpawn", false);
        isAttacking = false;

        yield return new WaitForSeconds(2f);
        isWaiting = false;

        isChasing = true;
        castSkil = false;

        animator.SetBool("IsRun", true);
        lastIsRunState = true;           //Cập nhật trạng thái

    }


    private IEnumerator isDead()
    {
        isChasing = false;
        isAttacking = false;
        isWaiting = true;
        animator.SetBool("IsRun", false);
        animator.SetBool("IsDead", true);
        yield return new WaitForSeconds(5f);
        Destroy(gameObject); 
    }

    



    IEnumerator SpawnRadaCheckMagic()
    {
        
        MagicAreaCheck.SetActive(true);
        yield return new WaitForSeconds(3f);
        MagicAreaCheck.SetActive(false);
        
        normalAtkCount = 0;
    }
}
