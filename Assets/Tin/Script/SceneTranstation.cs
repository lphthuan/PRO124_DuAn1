using System.Collections;
using UnityEngine;

public abstract class SceneTranstation : MonoBehaviour
{
    public abstract IEnumerator AnimateTransitionIn();
    public abstract IEnumerator AnimateTransitionOut();
}