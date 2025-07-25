using UnityEngine;

public class CastSkillArcher : MonoBehaviour
{
    [SerializeField] private GameObject areaMagicPrefab; // Prefab vùng triệu hồi
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public Transform groundTransform;
    float offsetY = 18f;
    public void CastSkill()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (groundTransform == null)
            groundTransform = GameObject.FindGameObjectWithTag("Ground")?.transform;

        if (playerTransform != null && groundTransform != null)
        {
            // Lấy vị trí player, nhưng Y là ground
            Vector3 playerPos = playerTransform.position;
            Vector3 groundPos = groundTransform.position;
            Vector3 spawnPos = new Vector3(playerPos.x, groundPos.y + offsetY, playerPos.z);

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