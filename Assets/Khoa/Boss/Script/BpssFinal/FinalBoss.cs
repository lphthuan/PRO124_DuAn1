using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public Animator animator;
    public int phaseFinalBoss = 0;
    [SerializeField] GameObject monster;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void phase1BossFinal()
    {
        animator.SetBool("Phase1", true);
    }

    public void  

}
