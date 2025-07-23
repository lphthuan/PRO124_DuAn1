using UnityEngine;

public class CastSkillArcher : MonoBehaviour
{
    [SerializeField] private GameObject areaMagicPrefab; // Prefab vùng triệu hồi
    [HideInInspector] public Transform playerTransform;

    public void CastSkill()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform != null)
        {
            Vector3 spawnPos = playerTransform.position + new Vector3(0, 6f, 0);

            GameObject magicArea = Instantiate(areaMagicPrefab, spawnPos, Quaternion.identity);

            SpawnAreaMagic spawnScript = magicArea.GetComponent<SpawnAreaMagic>();
            if (spawnScript != null)
            {
                spawnScript.magicPos = spawnPos;
            }

            Debug.Log("Skill triệu hồi vùng cảnh báo!");
            Destroy(magicArea, 5f);
        }
    }
}