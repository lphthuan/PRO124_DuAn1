using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBSpawnMonster : MonoBehaviour
{
    private float toaDoY = -1f;
    [SerializeField] GameObject SpawnMonster;
    public void SpawnModCollider()
    {
        Vector3 spawnMonster = transform.position;
        spawnMonster.y += toaDoY;
        GameObject monster = Instantiate(SpawnMonster, spawnMonster, Quaternion.identity);
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}

