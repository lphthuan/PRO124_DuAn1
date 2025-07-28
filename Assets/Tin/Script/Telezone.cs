using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telezone : MonoBehaviour
{
    private BossMovement bossMovement;
    // Start is called before the first frame update
    void Start()
    {
        bossMovement = GetComponent<BossMovement>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public Vector3 teleportPosition = new Vector3(252.32f, -3.41f, 0f);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = teleportPosition;
        }
    }
}
