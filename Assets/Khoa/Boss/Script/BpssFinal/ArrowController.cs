using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour
{
    public float gravityInitial = 0.01f;
    public float gravityAfter = 30f;
    public float delayGravity = 2f;

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
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}