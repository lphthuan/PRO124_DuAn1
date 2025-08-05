using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject door; 
    private bool objectOnPlate = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Movable"))
        {
            objectOnPlate = true;

            if (door != null)
                door.SetActive(false); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Movable"))
        {
            objectOnPlate = false;

            if (door != null)
                door.SetActive(true); 
        }
    }
}
