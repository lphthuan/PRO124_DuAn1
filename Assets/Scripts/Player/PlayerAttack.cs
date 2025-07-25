using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float spellSpeed = 12f;
    [SerializeField] private float attackAngle = 120f;
    [SerializeField] private float attackCooldown = 0.8f;

    public SpellData currentSpell;

    private float lastAttackTime = -Mathf.Infinity;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool IsValidAttackAngle()
    {
        if (currentSpell == null) return false;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePos - firePoint.position);
        direction.z = 0f;
        Vector2 dirNormalized = direction.normalized;

        // Dựa trên flipX để xác định hướng nhìn
        bool facingRight = !spriteRenderer.flipX;
        Vector2 facing = facingRight ? Vector2.right : Vector2.left;

        // Góc giữa hướng nhìn và hướng chuột
        float angle = Vector2.Angle(facing, dirNormalized);

        //Debug.Log($"[Attack] Facing: {(facingRight ? "Right" : "Left")} | Angle to mouse: {angle}");

        return angle <= attackAngle / 2f;
    }

    public void PerformAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        if (currentSpell == null) return;
        if (!IsValidAttackAngle()) return;

        lastAttackTime = Time.time;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePos - firePoint.position);
        direction.z = 0f;
        Vector2 dirNormalized = direction.normalized;

        // Tính góc để quay hướng phép về phía chuột
        float zRotation = Mathf.Atan2(dirNormalized.y, dirNormalized.x) * Mathf.Rad2Deg;

        // Tạo spell ở vị trí bắn, xoay đúng hướng
        GameObject spell = Instantiate(currentSpell.spellPrefab, firePoint.position, Quaternion.Euler(0, 0, zRotation));

        // Đẩy spell theo hướng chuột
        Rigidbody2D rb = spell.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = dirNormalized * spellSpeed;

        // Gửi hướng cho phép nếu có hiệu ứng gió
        var wind = spell.GetComponent<PlayerWindSpell>();
        if (wind != null)
            wind.SetDirection(dirNormalized);
    }
}
