using UnityEngine;

public class BeeBullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;

    private Transform player;
    private Vector2 targetDirection;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
        {
            targetDirection = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));
            transform.rotation = targetRotation;
        }
        else
        {
            targetDirection = transform.right;
        }

        rb.velocity = targetDirection * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("PlayerBullet"))
        {
            
            Destroy(gameObject);         
        }
        else if (!collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
