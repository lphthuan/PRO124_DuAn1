using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private BossMovement bossScript;
    // Start is called before the first frame update
    void Start()
    {
        bossScript = GetComponentInParent<BossMovement>();
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Gọi hàm từ script cha (boss) khi va chạm
            bossScript.OnHitPlayer(other.gameObject);
        }
    }
}
