using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDmgCollider : MonoBehaviour
{
    private float toaDoY = -0.5f;
    [SerializeField] GameObject MagicDmgArea;
    public void SpawnDamageCollider()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.y += toaDoY;
        GameObject vungSatThuong = Instantiate(MagicDmgArea, spawnPos, Quaternion.identity);
        Destroy(vungSatThuong, 0.3f);
    }
}
