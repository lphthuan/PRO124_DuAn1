using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CrossFade : SceneTranstation
{
    public CanvasGroup crossFade;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // giữ lại khi load scene
    }

    public override IEnumerator AnimateTransitionIn()
    {
        if (crossFade == null) yield break;

        crossFade.alpha = 0f;
        crossFade.gameObject.SetActive(true);

        yield return crossFade.DOFade(1f, 6f).WaitForCompletion();
    }

    public override IEnumerator AnimateTransitionOut()
    {
        if (crossFade == null) yield break;

        yield return crossFade.DOFade(0f, 0f).WaitForCompletion();

        crossFade.gameObject.SetActive(false);
    }
}
