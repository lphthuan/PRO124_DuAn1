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
    // Start is called before the first 
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();
        animatorController();
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
        if (isChasing == true) 
        {
         animator.SetBool("IsRun", true);
        }
        else
        {
            animator.SetBool("IsRun", false);
            
        }
    }
}
