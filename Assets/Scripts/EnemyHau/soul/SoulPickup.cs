using UnityEngine;

public class SoulPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoulUIManager.instance.AddSoul(1);  
            Destroy(gameObject);               
        }
    }
}
