using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public Vector3 pointC;
    public float speedAB = 0.2f;
    public float speedBC = 10f;

    private int phase = 0; // 0: A->B, 1: B->C

    void Start()
    {
        transform.position = pointA;
    }

    void Update()
    {
        if (phase == 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointB, speedAB * Time.deltaTime);
            if (Vector3.Distance(transform.position, pointB) < 0.01f)
            {
                phase = 1;
            }
        }
        else if (phase == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointC, speedBC * Time.deltaTime);
            // Xử lý khi tới điểm C nếu cần
        }
    }
}
