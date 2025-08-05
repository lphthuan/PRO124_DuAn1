using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    
    private bool doorDestroyed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!doorDestroyed && other.CompareTag("Movable"))
        {
            GameObject door = GameObject.FindWithTag("Door");
            if (door != null)
            {
                Destroy(door);
                doorDestroyed = true;
            }
        }
    }
}
