using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 6f;
    public int damage = 10;
    public float lifeTime = 3f;
    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifeTime); // tự hủy sau thời gian
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // Xoay viên đạn theo hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Gây sát thương nếu có script PlayerHealth
            //PlayerHealth player = collision.GetComponent<PlayerHealth>();
            //if (player != null)
            //{
            //    player.TakeDamage(damage);
            //}
            Destroy(gameObject);
        }
        else if (!collision.isTrigger) // chạm tường hoặc đối tượng khác
        {
            Destroy(gameObject);
        }
    }
}
