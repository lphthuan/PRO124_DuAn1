using UnityEngine;

public class SoulStone : MonoBehaviour
{
    public float pushForce = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("WindSpell"))
        {
            Vector2 pushDir = (transform.position - other.transform.position).normalized;
            GetComponent<Rigidbody2D>().AddForce(pushDir * pushForce, ForceMode2D.Impulse);
        }
    }
}
