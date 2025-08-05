using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TeleZone5 : MonoBehaviour
{
    [SerializeField] GameObject spawnTriggerTrap;
    [Header("Portal Settings")]
    public Transform targetPortal;
    public string promptMessage = "Press E to teleport";

    [Header("UI Settings")]
    public TMP_Text promptText;
    public Image fadeImage;

    private bool isPlayerInRange = false;
    private Transform player;
    private bool isTeleporting = false;

    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && !isTeleporting && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(TeleportWithFade());
            Vector3 spawnPos = new Vector3(transform.position.x , transform.position.y, transform.position.z);
            Instantiate(spawnTriggerTrap, spawnPos, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            if (!isTeleporting && promptText != null)
            {
                promptText.text = promptMessage;
                promptText.gameObject.SetActive(true);
            }
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptText != null)
                promptText.gameObject.SetActive(false);

            isPlayerInRange = false;
        }
    }

    private IEnumerator TeleportWithFade()
    {
        isTeleporting = true;

        if (promptText != null)
            promptText.gameObject.SetActive(false); // Ẩn chữ

        // Fade to black hoàn toàn
        yield return StartCoroutine(Fade(0f, 1f, 0.5f));

        // Khi đã tối hoàn toàn -> teleport
        if (player != null && targetPortal != null)
            player.position = targetPortal.position;

        // Reset velocity để không bị rơi hoặc bay tiếp
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Đợi 0.5 giây trước khi fade sáng
        yield return new WaitForSeconds(0.5f);

        // Fade từ từ sáng lại
        yield return StartCoroutine(Fade(1f, 0f, 0.5f));

        isTeleporting = false;

        // Nếu player vẫn ở trong vùng trigger thì hiện lại chữ
        if (isPlayerInRange && promptText != null)
        {
            promptText.text = promptMessage;
            promptText.gameObject.SetActive(true);
        }
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeImage == null)
            yield break;

        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        // Đảm bảo alpha về đúng đích
        fadeImage.color = new Color(c.r, c.g, c.b, to);
    }
}
