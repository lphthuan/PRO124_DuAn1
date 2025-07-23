using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamIntroBoss : MonoBehaviour
{
    public CinemachineVirtualCamera bossIntroCamera;
    public CinemachineVirtualCamera defaultCamera;
    public float returnDelay = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraManager.SwitchCamera(bossIntroCamera);

            StartCoroutine(ReturnToDefaultAfterDelay());
        }
    }

    private IEnumerator ReturnToDefaultAfterDelay()
    {
        yield return new WaitForSeconds(returnDelay);

        Destroy(gameObject);

        CameraManager.SwitchCamera(defaultCamera);
    }
}

