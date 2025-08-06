using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger2 : MonoBehaviour
{
    
    [SerializeField] private TrapActive trapScript;


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            trapScript.StartTrapCycle();
            
        }
    }
}
