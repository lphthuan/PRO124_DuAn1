using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public Transform Player;
    public float speed = 5f;
    public float PlayerCheckRange = 10f;
    private bool isChasing = false;
    public Animator animator;
    private bool lastIsRunState = false;
    // Start is called before the first 
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();
        animatorController();
        if(isChasing)
        {
            MoveToPlayer();
        }
    }


    public void DetectPlayer()
    {
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

    public void animatorController()
    {
        if (isChasing != lastIsRunState) 
        {
         animator.SetBool("IsRun", isChasing);
         lastIsRunState = isChasing;
        }
    }

    public void MoveToPlayer()
    {
        // Tạo một vector đích chỉ thay đổi trục X, giữ nguyên Y
        Vector3 targetPosition = new Vector3(Player.position.x, transform.position.y, transform.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;


        if (direction.x > 0)
        {
            // Player ở bên phải boss, hướng mặt sang phải
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0)
        {
            // Player ở bên trái boss, hướng mặt sang trái (lật sprite)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }


}
