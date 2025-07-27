using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyBossZone : MonoBehaviour
{
    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}
