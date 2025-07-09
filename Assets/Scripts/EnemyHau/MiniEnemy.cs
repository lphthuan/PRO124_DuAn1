using UnityEngine;

public class MiniEnemy : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 2f;
    public float dieDelay = 0.8f;
    public float detectionRange = 5f;

    private Transform player;
    private Animator animator;
    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();

        if (player == null)
        {
            Debug.LogWarning("Player not found by MiniEnemy!");
        }
    }

    void Update()
    {
        if (isDead || isAttacking || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > detectionRange) return; 

        
        Vector2 dir = (player.position - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);

       
        Vector3 scale = transform.localScale;
        scale.x = (player.position.x < transform.position.x) ? -1 : 1;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");

            other.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject, dieDelay);
        }
        else if (other.CompareTag("PlayerBullet"))
        {
            isDead = true;
            animator.SetTrigger("Hit");
            Destroy(other.gameObject);
            Invoke(nameof(Die), 0.4f);
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        Destroy(gameObject, dieDelay);
    }
}
