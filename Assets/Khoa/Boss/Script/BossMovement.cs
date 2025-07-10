using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [SerializeField] private GameObject dmgArea;
    public Transform Player;
    public float speed = 5f;
    public float PlayerCheckRange = 10f;
    private bool isChasing = false;
    public Animator animator;
    public Transform visualTransform;
    private bool lastIsRunState = false;
    private bool isAttacking = false; //Boss đang Attack
    private bool isWaiting = false;   //Boss đang cooldown
    public float HPBoss = 20f;

    void Update()
    {
        if (isWaiting || isAttacking)
        {
            animator.SetBool("IsRun", false); //Đứng yên trong khi tấn công/cooldown
            return;
        }

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
            return; //Nếu đang tấn công, không kiểm tra Player

        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        if (distanceToPlayer <= PlayerCheckRange)
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

    public void OnHitPlayer(GameObject playerObj)
    {
        if (!isAttacking && !isWaiting) 
        {
            StartCoroutine(AttackCoroutine());
        }
    }



    public void SpawnAttackCollider()
    {

        float huongQuai = (visualTransform != null) ? Mathf.Sign(visualTransform.localScale.x) : Mathf.Sign(transform.localScale.x);

        Vector3 offset = new Vector3(5 * huongQuai, 1, 0);
        GameObject vungSatThuong = Instantiate(dmgArea, transform.position + offset, Quaternion.identity);

        if (huongQuai < 0)
        {
            vungSatThuong.transform.localScale = new Vector3(-1, 1, 1);
        }

        Destroy(vungSatThuong, 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet") && HPBoss > 0)
        {
            HPBoss -= 5f;
        }
        if (HPBoss <= 0)
        {
            StartCoroutine(isDead());
        }
    }


    private IEnumerator AttackCoroutine()
    {

        isChasing = false;
        isAttacking = true;
        isWaiting = true;

        animator.SetBool("IsRun", false);
        animator.SetBool("IsAtk", true);


        yield return new WaitForSeconds(0.8f);

        animator.SetBool("IsAtk", false);   //quay về Idle
        isAttacking = false;                //cho phép nhận Attack tiếp theo


        yield return new WaitForSeconds(1f);
        isWaiting = false;

        isChasing = true; //boss có thể rượt tiếp
    }


    private IEnumerator isDead()
    {
        isChasing = false;
        isAttacking = false;
        isWaiting = true;
        animator.SetBool("IsRun", false);
        animator.SetBool("IsDead", true);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject); // Xóa Boss sau khi chết
    }
}
