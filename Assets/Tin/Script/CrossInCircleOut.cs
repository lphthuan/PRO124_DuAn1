using System.Collections;
using UnityEngine;

public class CrossInCircleOut : SceneTranstation
{
    [SerializeField] private CrossFade crossFade;
    [SerializeField] private CircleWipe circleWipe;

    private void Awake()
    {
        crossFade.gameObject.SetActive(false);
        DontDestroyOnLoad(gameObject); // giữ lại qua scene
    }

    public override IEnumerator AnimateTransitionIn()
    {
        if (crossFade != null)
            yield return crossFade.AnimateTransitionIn();
    }

    public override IEnumerator AnimateTransitionOut()
    {
        if (circleWipe != null)
            yield return circleWipe.AnimateTransitionOut();
    }
}
