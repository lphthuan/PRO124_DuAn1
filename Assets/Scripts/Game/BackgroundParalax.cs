using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
	[System.Serializable]
	public class ParallaxLayer
	{
		public Transform layerTransform;
		public float parallaxFactor = 0.5f; // 1 = di chuyển cùng tốc độ camera, 0 = đứng yên
	}

	[Header("Parallax Settings")]
	[SerializeField] private ParallaxLayer[] layers;
	[SerializeField] private Transform cameraTransform;

	private Vector3 previousCamPos;

	private void Start()
	{
		if (cameraTransform == null)
			cameraTransform = Camera.main.transform;

		previousCamPos = cameraTransform.position;
	}

	private void LateUpdate()
	{
		Vector3 deltaMovement = cameraTransform.position - previousCamPos;

		foreach (var layer in layers)
		{
			if (layer.layerTransform != null)
			{
				Vector3 move = deltaMovement * layer.parallaxFactor;
				layer.layerTransform.position += new Vector3(move.x, move.y, 0f);
			}
		}

		previousCamPos = cameraTransform.position;
	}
}
