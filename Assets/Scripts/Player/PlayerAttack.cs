using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack Instance { get; private set; }

    [Header("Attack Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float spellSpeed = 12f;
    [SerializeField] private float attackAngle = 120f;
    [SerializeField] private float attackCooldown = 0.8f;
    public float baseDamage = 10f;
    private float currentDamage;

    public SpellData currentSpell;

    private float lastAttackTime = -Mathf.Infinity;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        Instance = this;
        currentDamage = baseDamage;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerDamage"))
        {
            baseDamage = PlayerPrefs.GetFloat("PlayerDamage");
        }
    }


    public float GetDamage()
    {
        return currentDamage;
    }

    public void SetDamage(float newDamage)
    {
        currentDamage = newDamage;
        Debug.Log($"[Player] Damage cập nhật: {currentDamage}");
    }

    public void BoostDamage(float amount, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DamageBoostCoroutine(amount, duration));
    }

    private IEnumerator DamageBoostCoroutine(float amount, float duration)
    {
        currentDamage += amount;
        Debug.Log($"🟢 Damage boosted: {currentDamage}");

        yield return new WaitForSeconds(duration);

        currentDamage -= amount;
        Debug.Log($"🔴 Damage boost ended: {currentDamage}");
    }

    /*public bool IsValidAttackAngle()
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
    }*/

    public void PerformAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        if (currentSpell == null) return;

        // 👉 Quay mặt theo hướng chuột
        FlipToMouse();

        lastAttackTime = Time.time;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePos - firePoint.position);
        direction.z = 0f;
        Vector2 dirNormalized = direction.normalized;

        float zRotation = Mathf.Atan2(dirNormalized.y, dirNormalized.x) * Mathf.Rad2Deg;

        GameObject spell = Instantiate(
            currentSpell.spellPrefab,
            firePoint.position,
            Quaternion.Euler(0, 0, zRotation)
        );

        Rigidbody2D rb = spell.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = dirNormalized * spellSpeed;

        var wind = spell.GetComponent<PlayerWindSpell>();
        if (wind != null)
            wind.SetDirection(dirNormalized);
    }

    private void FlipToMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouseWorldPos.x < transform.position.x)
            spriteRenderer.flipX = true;   // Quay trái
        else
            spriteRenderer.flipX = false;  // Quay phải
    }
}