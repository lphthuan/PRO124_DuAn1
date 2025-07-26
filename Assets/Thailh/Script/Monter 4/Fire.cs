using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float lifetime = 5f;
    public int damage = 5;
    public float speed = 3f; // tốc độ rượt theo
    public float delayTime = 0f; // thời gian đứng yên trước khi rượt
    private Transform player;
    private bool canMove = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        StartCoroutine(StartChasing()); // bắt đầu chờ

        Destroy(gameObject, lifetime);
    }

    IEnumerator StartChasing()
    {
        yield return new WaitForSeconds(delayTime); // đợi 1 giây
        canMove = true; // cho phép di chuyển
    }

    void Update()
    {
        if (canMove && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        else if (!collision.CompareTag("enemy") && !collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
