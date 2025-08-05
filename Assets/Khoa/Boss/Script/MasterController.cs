using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour
{
    [SerializeField] GameObject MasterHit;



    public void MasterAttack()
    {
        Vector3 spawnPos1 = new Vector3(transform.position.x + 5f, transform.position.y, transform.position.z);
        GameObject SummonArrow1 = Instantiate(MasterHit, spawnPos1, Quaternion.identity);
        Destroy(SummonArrow1, 0.2f);
    }
}
