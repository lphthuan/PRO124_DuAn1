using UnityEngine;
using System.Collections;

public class PlayerWindSpell : MonoBehaviour
{
	[SerializeField] private float knockbackForce = 8f;
	[SerializeField] private float knockbackXPower = 1f; // sức bật ngang
	[SerializeField] private float knockbackYPower = 0.3f; // hất lên nhẹ
	[SerializeField] private float disableTime = 0.5f;

	private Vector2 direction;

	public void SetDirection(Vector2 dir)
	{
		direction = dir.normalized;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag("Enemy")) return;

		Rigidbody2D rb = other.attachedRigidbody;
		if (rb == null || rb.bodyType != RigidbodyType2D.Dynamic) return;

		// Tắt AI quái (nếu có)
		MonoBehaviour monster = other.GetComponent("Monster") as MonoBehaviour;
		MonoBehaviour monster4 = other.GetComponent("Monster4") as MonoBehaviour;

		if (monster != null)
		{
			monster.enabled = false;
			StartCoroutine(ReenableScript(monster, disableTime));
		}
		else if (monster4 != null)
		{
			monster4.enabled = false;
			StartCoroutine(ReenableScript(monster4, disableTime));
		}

		// 🌀 Knockback chỉ thiên về ngang
		Vector2 knockDir = new Vector2(Mathf.Sign(direction.x) * knockbackXPower, knockbackYPower).normalized;

		rb.velocity = Vector2.zero;
		rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);

		Debug.Log($"[WindSpell] Knockback applied to {other.name} with dir {knockDir}");
		Destroy(gameObject,0.2); // Xóa spell sau khi tác động
	}

	private IEnumerator ReenableScript(MonoBehaviour script, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (script != null) script.enabled = true;
	}
}
