using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
   
    [SerializeField] GameObject trap;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            Vector3 transform = new Vector3(-41.71f, -110.48f, 0f);
            Instantiate(trap, transform, Quaternion.identity);
            Destroy(gameObject); 
        }
    }
}

