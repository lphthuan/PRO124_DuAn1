using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class TrapMoveUp : MonoBehaviour
{
    public int timing = 0;
    public int winCheck = 0;
    public float riseSpeed = 1.5f; // Tốc độ di chuyển lên
    private bool isActivated = false;
    private void Start()
    {
        InvokeRepeating(nameof(IncreaseA), 2f, 41f);
        Destroy(gameObject, 60f);
    }

    private void Update()
    {
        if (timing == 0)
        {
            return; // Không làm gì nếu timing bằng 0
        }
        if (timing > 2)
        {
            winCheck = 2;
            return;
        }
        else if (timing == 2)
        {
            winCheck = 1;
            timing = 3;
            return;
        }


        else if (timing == 1)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }

    }
    void IncreaseA()
    {
        timing += 1;
        Debug.Log("timing = " + timing);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            StartCoroutine(TelePlayer(collision.gameObject));
        }
    }
    private IEnumerator TelePlayer(GameObject player)
    {
        yield return new WaitForSeconds(2f);
        Vector3 teleportPosition = new Vector3(-37.02f, 35.12f, 0f);
        player.transform.position = teleportPosition;
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}