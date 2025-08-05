using UnityEngine;

public class SoulStone : MonoBehaviour
{
    public float pushForce = 5f;
    private Vector3 initialPosition;
    private Rigidbody2D rb;

    private void Start()
    {
        
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WindSpell"))
        {
            Vector2 pushDir = (transform.position - other.transform.position).normalized;
            rb.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
        }
        else if (other.CompareTag("Limit"))
        {
            
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            transform.position = initialPosition;
        }
    }
}
