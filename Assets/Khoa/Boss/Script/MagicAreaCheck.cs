using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAreaCheck : MonoBehaviour
{
    private BossMovement bossScript;
    // Start is called before the first frame update
    void Start()
    {
        bossScript = GetComponentInParent<BossMovement>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {

    }
}
