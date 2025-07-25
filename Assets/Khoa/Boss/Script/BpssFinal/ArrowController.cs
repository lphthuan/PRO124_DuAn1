using UnityEngine;
using System.Collections;
using FirstGearGames.SmoothCameraShaker;

public class ArrowController : MonoBehaviour
{
    [Header("Shake settings")]
    public ShakeData shakeData; // Kéo ShakeData preset vào đây

    public float gravityInitial = 0.01f;
    public float gravityAfter = 15f;
    public float delayGravity = 2f;
    [SerializeField] GameObject arrowDmgArea;
    private Rigidbody2D rb2d;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        // Đặt Rigidbody2D luôn ở Dynamic, gravity ban đầu nhỏ
        if (rb2d != null)
        {
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            rb2d.gravityScale = gravityInitial;
            rb2d.velocity = Vector2.zero;
        }

        // Sau delayGravity giây, tăng gravityScale lên giá trị lớn
        StartCoroutine(IncreaseGravityCoroutine());
    }

    IEnumerator IncreaseGravityCoroutine()
    {
        yield return new WaitForSeconds(delayGravity);
        if (rb2d != null)
        {
            rb2d.gravityScale = gravityAfter;
            Debug.Log("[Arrow] Gravity tăng lên " + gravityAfter);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Tạo vị trí mới với Y cộng thêm 5
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 6f, transform.position.z);
            Instantiate(arrowDmgArea, spawnPos, Quaternion.identity);
            Destroy(gameObject);
            // Gọi rung camera
            if (CameraShaker.Instance != null && shakeData != null)
            {
                CameraShaker.Instance.Shake(shakeData);
            }
        }
    }

}