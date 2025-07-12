using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class ASyncLoading : MonoBehaviour
{
    public static ASyncLoading Instance;

    [Header("Menu UI")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;

    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private Text ValueText;

    [Header("Tips")]
    [SerializeField] private Text tipsText;
    [SerializeField][TextArea] private string[] tipsArray;

    [Header("Transitions")]
    [SerializeField] private GameObject transitionsContainer;

    private SceneTranstation[] transitions;
    private int currentTipIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        transitions = transitionsContainer.GetComponentsInChildren<SceneTranstation>(true);
    }

    public void PlayGameBtn(int sceneIndex, string transitionName)
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);

        currentTipIndex = 0;
        StartCoroutine(ShowTipsLoop());
        StartCoroutine(LoadSceneAsync(sceneIndex, transitionName));
    }

    public void PlaySceneFadeBlack(int sceneIndex)
    {
        PlayGameBtn(sceneIndex, "CrossCircle");
    }

    IEnumerator LoadSceneAsync(int sceneIndex, string transitionName)
    {
        SceneTranstation transition = transitions.First(t => t.name == transitionName);

        yield return transition.AnimateTransitionIn();

        loadingSlider.value = 0f;
        loadingSlider.gameObject.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (operation.progress < 0.9f)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime * 0.2f);

            loadingSlider.value = displayedProgress;
            ValueText.text = Mathf.RoundToInt(displayedProgress * 100f) + "%";
            yield return null;
        }

        while (displayedProgress < 0.995f)
        {
            displayedProgress = Mathf.MoveTowards(displayedProgress, 1f, Time.deltaTime * 0.2f);
            loadingSlider.value = displayedProgress;
            ValueText.text = Mathf.RoundToInt(displayedProgress * 100f) + "%";
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        StopCoroutine("ShowTipsLoop");

        operation.allowSceneActivation = true;

        loadingSlider.gameObject.SetActive(false);

        yield return transition.AnimateTransitionOut();
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
