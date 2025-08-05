using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonsterKhoa : MonoBehaviour
{
    private float toaDoY = -1f;
    [SerializeField] GameObject SpawnMod;
    public void SpawnModCollider()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.y += toaDoY;
        GameObject vungSatThuong = Instantiate(SpawnMod, spawnPos, Quaternion.identity);
        
    }
}
