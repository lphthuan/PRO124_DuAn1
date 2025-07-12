using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CircleWipe : SceneTranstation
{
    public Image circle;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // giữ lại khi load scene
    }

    public override IEnumerator AnimateTransitionIn()
    {
        circle.rectTransform.anchoredPosition = new Vector2(-1000f, 0f);
        var tweener = circle.rectTransform.DOAnchorPosX(0f, 1f);
        yield return tweener.WaitForCompletion();
    }

    public override IEnumerator AnimateTransitionOut()
    {
        var tweener = circle.rectTransform.DOAnchorPosX(1000f, 1.5f);
        yield return tweener.WaitForCompletion();
    }
}