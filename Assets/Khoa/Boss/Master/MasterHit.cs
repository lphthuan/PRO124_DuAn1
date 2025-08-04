using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterHit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Shield"))
        {
            // Tìm PlayerController từ object có Shield
            PlayerController playerController = collision.GetComponent<shieldCode>()?.playerController;
            if (playerController != null)
            {
                playerController.shieldCheck = false;
                Debug.Log("Đã tắt khiên từ Master Attack!");
            }
        }
    }
}
