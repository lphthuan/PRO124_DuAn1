using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyBossZone : MonoBehaviour
{
    [SerializeField] GameObject vortex;
    public void summonVortex()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);
        Instantiate(vortex, spawnPos, Quaternion.identity);
    }
    public void OnDestroy()
    {
        
        Destroy(gameObject);
    }
}
