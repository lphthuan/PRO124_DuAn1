using UnityEngine;

public class SpawnAreaMagic : MonoBehaviour
{
    public Vector3 magicPos; 
    [SerializeField] private GameObject arrowPrefab;

    // Hàm gọi qua Animation Event
    public void SpawnArrow()
    {
        Vector3 magicPos = new Vector3(transform.position.x - 0.2f, transform.position.y + 2f, transform.position.z);
        GameObject arrow = Instantiate(arrowPrefab, magicPos, Quaternion.identity);

        ArrowController ctrl = arrow.GetComponent<ArrowController>();
        if (ctrl != null)
        {
            ctrl.gravityInitial = 0.1f;   // Gravity ban đầu
            ctrl.gravityAfter = 15f;      // Gravity sau delay
            ctrl.delayGravity = 2f;       // Thời gian delay trước khi tăng gravity
        }
    }
}