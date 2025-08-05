using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VongXoay : MonoBehaviour
{
    public int sceneIndexToLoad = 1;
    public Transform player;
    public float suckSpeed = 5f;
    public float rotateSpeed = 180f; // độ/giây
    private Rigidbody2D playerRb;
    private float originalGravityScale;

    private MonoBehaviour[] playerScripts; // chứa các script điều khiển Player

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
        {
            // Lấy Rigidbody
            playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                originalGravityScale = playerRb.gravityScale;
                playerRb.gravityScale = 0f;
            }

            // Tắt tất cả script trên Player (trừ Transform, Rigidbody, Animator nếu cần)
            playerScripts = player.GetComponents<MonoBehaviour>();
            foreach (var script in playerScripts)
            {
                // Chỉ disable script không phải là VongXoay và không null
                if (script != null && script != this)
                    script.enabled = false;
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Hướng hút vào tâm vòng xoáy
            Vector3 direction = (transform.position - player.position).normalized;

            // Dịch chuyển Player
            player.position += direction * suckSpeed * Time.deltaTime;

            // Xoay Player quanh trục Z
            player.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
    }

    // (Tùy chọn) Gọi hàm này để khôi phục sau khi hút
    public void RestorePlayerState()
    {
        if (playerRb != null)
            playerRb.gravityScale = originalGravityScale;

        foreach (var script in playerScripts)
        {
            if (script != null)
                script.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(loadScenes());
        }
    }
    private IEnumerator loadScenes() 
    {
        yield return new WaitForSeconds(2.4f);
        //chuyển sang scene khác
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map2");
    }

}
