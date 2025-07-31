using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingTextController : MonoBehaviour
{
    public TMP_Text upgradeText;
    public float floatUpSpeed = 0.5f;
    public float lifetime = 2f;
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    private void OnEnable()
    {
        StartCoroutine(FadeAndDestroy());
    }

    public void ShowText(string message)
    {
        upgradeText.text = message;
        transform.localPosition = offset;
    }

    IEnumerator FadeAndDestroy()
    {
        float timer = 0f;
        Vector3 startPos = transform.localPosition;
        Color startColor = upgradeText.color;

        while (timer < lifetime)
        {
            timer += Time.unscaledDeltaTime;

            // Di chuyển text lên từ từ
            transform.localPosition = startPos + Vector3.up * floatUpSpeed * timer;

            // Fade out dần
            float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
            upgradeText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        gameObject.SetActive(false); // Tắt sau khi hiện xong
    }
}
