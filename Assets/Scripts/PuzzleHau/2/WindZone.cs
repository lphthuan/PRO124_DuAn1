using UnityEngine;

public class WindZone : MonoBehaviour
{
    public float pushForce = 10f;
    private bool windActive = true;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (windActive && other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Vector2.left * pushForce);
            }
        }
    }

    public void StopWind()
    {
        
        Destroy(gameObject); 
    }
}
