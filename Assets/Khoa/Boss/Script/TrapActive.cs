using System.Collections;
using UnityEngine;

public class TrapActive : MonoBehaviour
{
    [SerializeField] private GameObject trapVisual; // GameObject chứa collider/hiển thị trap

    public void StartTrapCycle()
    {
        StartCoroutine(ActiveGameObject());
        Debug.Log("Trap is now active and will be deactivated after 30 seconds.");
    }

    private IEnumerator ActiveGameObject()
    {
        yield return new WaitForSeconds(45f);

            trapVisual.SetActive(false);

        yield return new WaitForSeconds(5f);

            trapVisual.SetActive(true);
    }
}
