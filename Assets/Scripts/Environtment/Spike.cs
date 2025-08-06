using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
	[Header("Collider cần bật/tắt khi đâm lên")]
	[SerializeField] private Collider2D damageCollider;

	private void Awake()
	{
		if (damageCollider == null)
		{
			damageCollider = GetComponent<Collider2D>();
		}

		damageCollider.enabled = false;
	}

	// Gọi từ animation event khi chông đâm lên
	public void EnableDamage()
	{
		damageCollider.enabled = true;
	}

	// Gọi từ animation event khi chông hạ xuống
	public void DisableDamage()
	{
		damageCollider.enabled = false;
	}
}
