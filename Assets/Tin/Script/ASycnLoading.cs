using System.Collections;
using System.Collections.Generic;
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

    public void PlayGameBtn(int sceneIndex)
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);

        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    IEnumerator LoadSceneAsync(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        operation.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (operation.progress < 0.9f)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Làm chậm bằng Lerp
            displayedProgress = Mathf.Lerp(displayedProgress, targetProgress, Time.deltaTime * 1f);

            loadingSlider.value = displayedProgress;
            ValueText.text = Mathf.RoundToInt(displayedProgress * 100f) + "%";

            yield return null;
        }

        // Đảm bảo tiến trình đạt 100%
        while (displayedProgress < 0.995f)
        {
            displayedProgress = Mathf.Lerp(displayedProgress, 1f, Time.deltaTime * 1f);
            loadingSlider.value = displayedProgress;
            ValueText.text = Mathf.RoundToInt(displayedProgress * 100f) + "%";

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        operation.allowSceneActivation = true;
    }

}

