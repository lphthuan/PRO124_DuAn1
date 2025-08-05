using UnityEngine;
using System.Collections;

public class TestTrigger : MonoBehaviour
{
    [Header("Trap detection")]
    public string trapTag = "Trap";

    [Header("TeleZone references (choose one)")]
    public GameObject teleZoneInstanceInScene;
    public GameObject teleZonePrefab;
    public bool usePrefab = false;

    [Header("Optional")]
    public float activateDelay = 0f;

    private bool alreadyTriggered = false;

    private void Reset()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyTriggered) return;
        if (other == null) return;
        if (!other.CompareTag(trapTag)) return;

        alreadyTriggered = true;

        // KHÔNG tắt gameObject ở đây!
        // Thay vào đó khởi chạy Coroutine xử lý
        StartCoroutine(HandleTrigger());
    }

    private IEnumerator HandleTrigger()
    {
        // 1) Kích hoạt TeleZone theo chế độ bạn chọn
        if (usePrefab)
        {
            if (teleZonePrefab != null)
            {
                // Instantiate tại vị trí Test hiện tại (thường hợp tốt)
                Instantiate(teleZonePrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("[TestTrigger] usePrefab = true nhưng teleZonePrefab chưa gán!");
            }
        }
        else
        {
            if (teleZoneInstanceInScene != null)
            {
                teleZoneInstanceInScene.SetActive(true);
            }
            else
            {
                Debug.LogWarning("[TestTrigger] usePrefab = false nhưng teleZoneInstanceInScene chưa gán!");
            }
        }

        if (activateDelay > 0f)
            yield return new WaitForSeconds(activateDelay);
        else
            yield return null; 

        gameObject.SetActive(false);
    }
}
