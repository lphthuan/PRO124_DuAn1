using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDmgCollider : MonoBehaviour
{
    private float toaDoY = -0.7f;
    [SerializeField] GameObject MagicDmgArea;
    public Vector3 spawnPos;
    public void SpawnDamageCollider()
    {
        spawnPos.y += toaDoY;
        GameObject vungSatThuong = Instantiate(MagicDmgArea, spawnPos, Quaternion.identity);
        Destroy(vungSatThuong, 0.3f);
    }
}
