using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastSkillArcher : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [HideInInspector] public Transform playerTransform;
    void Start()
    {
        if (arrowSpawnPoint == null)
        {
            arrowSpawnPoint = transform;
        }
    }
    public void spawnAreaMagic()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform != null)
        {
            // Lấy vị trí player, cộng offset lên trên đầu
            Vector3 spawnPos = playerTransform.position + new Vector3(0, 6f, 0);

            GameObject vungtrieuhoi = Instantiate(arrowPrefab, spawnPos, Quaternion.identity); // cần học chỗ này
            Debug.Log("Skill1 được kích hoạt!");
            Destroy(vungtrieuhoi, 5f);
        }
    }

}
