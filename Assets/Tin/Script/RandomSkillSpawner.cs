using UnityEngine;
using System.Collections.Generic;

public class RandomSkillSpawner : MonoBehaviour
{
    public GameObject skillPrefab;
    public int maxTries = 20;

    // Giới hạn vùng spawn
    public Vector2 minPosition = new Vector2(-5f, 0f);
    public Vector2 maxPosition = new Vector2(5f, 3f);

    // Lưu vị trí đã spawn để tránh trùng
    private List<Vector3> usedPositions = new List<Vector3>();
    public float minDistanceBetweenSpawns = 1.0f;

    public void SpawnSkill()
    {
        Vector3 spawnPos = Vector3.zero;
        bool found = false;

        for (int i = 0; i < maxTries; i++)
        {
            float x = Random.Range(minPosition.x, maxPosition.x);
            float y = Random.Range(minPosition.y, maxPosition.y);
            spawnPos = new Vector3(x, y, 0f);

            if (IsFarEnough(spawnPos))
            {
                found = true;
                break;
            }
        }

        if (found)
        {
            Instantiate(skillPrefab, spawnPos, Quaternion.identity);
            usedPositions.Add(spawnPos);
        }
        else
        {
            Debug.LogWarning("Không tìm được vị trí phù hợp sau nhiều lần thử.");
        }
    }

    // Kiểm tra khoảng cách giữa vị trí mới và tất cả skill đã spawn
    private bool IsFarEnough(Vector3 pos)
    {
        foreach (var used in usedPositions)
        {
            if (Vector3.Distance(pos, used) < minDistanceBetweenSpawns)
                return false;
        }
        return true;
    }
}
