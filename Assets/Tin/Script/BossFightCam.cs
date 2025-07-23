using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightCam : MonoBehaviour
{
    public CinemachineVirtualCamera bossFightCamera;    // Cam zoom vào boss
    public CinemachineVirtualCamera defaultCamera;      // Cam mặc định

    public GameObject wall1;
    public GameObject wall2;

    public GameObject BossHealth;

    private BossHealth boss;

    private void Start()
    {
        wall1.SetActive(false);
        wall2.SetActive(false);
        BossHealth.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {

            // Kích hoạt camera đánh boss
            CameraManager.SwitchCamera(bossFightCamera);

            //Bật wall
            StartCoroutine(Wall());

            // Tự động tìm boss trong scene
            boss = FindObjectOfType<BossHealth>();
            if (boss != null)
            {
                boss.OnBossDead += HandleBossDead;
            }
        }
    }

    private void HandleBossDead()
    {
        CameraManager.SwitchCamera(defaultCamera);

        Destroy(gameObject);
        Destroy(wall1);
        Destroy(wall2);

        boss.OnBossDead -= HandleBossDead; 
    }

    private void OnDestroy()
    {
        if (boss != null)
        {
            boss.OnBossDead -= HandleBossDead;
        }
    }

    private IEnumerator Wall()
    {
        yield return new WaitForSeconds(1);

        BossHealth.SetActive(true);

        //Kích hoạt wall
        wall1.SetActive(true);
        wall2.SetActive(true);
    }
}
