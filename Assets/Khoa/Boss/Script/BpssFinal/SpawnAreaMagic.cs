using UnityEngine;

public class SpawnAreaMagic : MonoBehaviour
{
    public Vector3 magicPos; 
    [SerializeField] private GameObject arrowPrefab;

    // Hàm gọi qua Animation Event
    public void SpawnArrow()
    {
        
        GameObject arrow = Instantiate(arrowPrefab, magicPos, Quaternion.identity);

        ArrowController ctrl = arrow.GetComponent<ArrowController>();
        if (ctrl != null)
        {
            ctrl.gravityInitial = 0.01f;   // Gravity ban đầu
            ctrl.gravityAfter = 15f;      // Gravity sau delay
            ctrl.delayGravity = 2f;       // Thời gian delay trước khi tăng gravity
        }
    }
}