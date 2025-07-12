using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ASyncLoading : MonoBehaviour
{
    [Header("Menu Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;

    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private Text ValueText;

    [Header("Tips")]
    [SerializeField] private Text tipsText;
    [SerializeField][TextArea] private string[] tipsArray;

    private int currentTipIndex = 0;

    public void PlayGameBtn(int sceneIndex)
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);

        // Bắt đầu hiện tips
        StartCoroutine(ShowTipsLoop());

        // Bắt đầu load scene
        StartCoroutine(PreLoadDelay(sceneIndex));
    }

    IEnumerator PreLoadDelay(int sceneIndex)
    {
        yield return null;
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    IEnumerator LoadSceneAsync(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        operation.allowSceneActivation = false;

        float displayedProgress = 0f;

        // Giai đoạn từ 0 đến 90%
        while (operation.progress < 0.9f)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime * 0.2f);

            loadingSlider.value = displayedProgress;
            ValueText.text = Mathf.RoundToInt(displayedProgress * 100f) + "%";
            yield return null;
        }

        // Giai đoạn 90 → 100%
        while (displayedProgress < 0.995f)
        {
            displayedProgress = Mathf.MoveTowards(displayedProgress, 1f, Time.deltaTime * 0.2f);

            loadingSlider.value = displayedProgress;
            ValueText.text = Mathf.RoundToInt(displayedProgress * 100f) + "%";
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        operation.allowSceneActivation = true;
    }

    IEnumerator ShowTipsLoop()
    {
        while (true)
        {
            if (tipsArray.Length > 0)
            {
                tipsText.text = "Tips:\n " + tipsArray[currentTipIndex];

                currentTipIndex = (currentTipIndex + 1) % tipsArray.Length;
            }

            yield return new WaitForSeconds(2f);
        }
    }
}
