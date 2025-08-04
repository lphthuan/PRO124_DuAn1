using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shieldCode : MonoBehaviour
{
    public bool parryWindow = false;
    public PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(DestroyAfterTime());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MasterBoss") || collision.CompareTag("Enemy") || collision.CompareTag("EnemyBullet") || collision.CompareTag("CauLuaQuai2"))
        {

            playerController.StartCoroutine(playerController.StartShieldCountdown());
            StartCoroutine(DelayedDisableShield());
            Destroy(gameObject);
        }
    }


    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(3f);
        playerController.shieldHave = false;
        playerController.shieldCheck = false;
        Destroy(gameObject);
    }
    private IEnumerator DelayedDisableShield()
    {
        yield return new WaitForSeconds(0.5f);
        playerController.shieldHave = false; // Player giờ có thể bị sát thương
    }
    public void EnableParryWindow()
    {
        parryWindow = true;
        Invoke("DisableParryWindow", 0.3f); // Cho phép parry trong 0.2 giây
    }

    private void DisableParryWindow()
    {
        parryWindow = false;
    }
}

