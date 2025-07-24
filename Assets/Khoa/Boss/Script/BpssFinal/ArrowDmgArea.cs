using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDmgArea : MonoBehaviour
{
    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}
