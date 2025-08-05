using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phase1BossController : MonoBehaviour
{
    private FinalBoss controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<FinalBoss>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            controller.phaseFinalBoss = 1;
            controller.animator.SetBool("IsPhase1", true);
            Destroy(gameObject);
        }
    }
}
