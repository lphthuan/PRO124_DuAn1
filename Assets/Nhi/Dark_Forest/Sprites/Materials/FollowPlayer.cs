using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;   // Kéo Player vào đây trong Inspector
    public Vector3 offset;     // Khoảng cách giữa background và player

    void Update()
    {
        // Cập nhật vị trí của background dựa theo vị trí của Player + offset
        transform.position = new Vector3(
            player.position.x + offset.x,
            player.position.y + offset.y,
            transform.position.z // Giữ nguyên chiều sâu (Z)
        );
    }
}
