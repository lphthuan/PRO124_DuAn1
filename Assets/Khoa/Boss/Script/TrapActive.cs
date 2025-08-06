using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapActive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ActiveGameObject());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private IEnumerator ActiveGameObject()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }
}
