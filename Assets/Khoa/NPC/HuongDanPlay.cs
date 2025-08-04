using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HuongDanPlay : MonoBehaviour
{
    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public GameObject npcText;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public string[] dialogueLines;
    public float typingSpeed = 0.05f;

    [Header("ChoicePanel")]
    public GameObject choicePanel;
    public Button Play;
    public Button Exit;

    [Header("Minigame")]
    public GameObject PanelEmoji;

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool isTalking = false;
    private bool isTyping = false;
    private bool hasTalked = false;

    private GameObject player;
    private MonoBehaviour[] playerScriptsToDisable;

    // Tham chiếu TMP_Text của npcText
    private TMP_Text npcTextTMP;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        npcTextTMP = npcText.GetComponent<TMP_Text>();

        npcText.SetActive(false);
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        if (PanelEmoji != null)
            PanelEmoji.SetActive(false);

        // Gán sự kiện nút
        Play.onClick.AddListener(OnPlayClicked);
        Exit.onClick.AddListener(OnExitClicked);
    }

    private void Update()
    {
        if (playerInRange && !isTalking && Input.GetKeyDown(KeyCode.E))
        {
            if (!hasTalked)
            {
                npcText.SetActive(false);
                StartDialogue();
            }
            else
            {
                StartCoroutine(ShowCantTalkMessage());
            }
        }

        if (isTalking && !isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }

    private void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        currentLine = 0;
        isTalking = true;
        DisablePlayerControl();
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (currentLine < dialogueLines.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeLine(dialogueLines[currentLine]));
        }
        else if (currentLine == dialogueLines.Length)
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
        currentLine++;
    }

    private void EndDialogue()
    {
        choicePanel.SetActive(true);
        hasTalked = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            playerScriptsToDisable = player.GetComponents<MonoBehaviour>();

            npcText.SetActive(true);

            if (!hasTalked)
                npcTextTMP.text = "Press E to talk";
            else
                npcTextTMP.text = ""; // không hiện gì ban đầu sau khi đã nói
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            npcText.SetActive(false);
        }
    }

    private void DisablePlayerControl()
    {
        if (playerScriptsToDisable != null)
        {
            foreach (var script in playerScriptsToDisable)
            {
                if (script != this && script.enabled)
                {
                    script.enabled = false;
                }
            }
        }
    }

    private void EnablePlayerControl()
    {
        if (playerScriptsToDisable != null)
        {
            foreach (var script in playerScriptsToDisable)
            {
                if (!script.enabled)
                {
                    script.enabled = true;
                }
            }
        }
    }

    // === NÚT PLAY ===
    public void OnPlayClicked()
    {
        choicePanel.SetActive(false);
        dialoguePanel.SetActive(false);

        if (PanelEmoji != null)
            PanelEmoji.SetActive(true);

        EnablePlayerControl();
    }

    // === NÚT EXIT ===
    public void OnExitClicked()
    {
        choicePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        EnablePlayerControl();
    }

    private IEnumerator ShowCantTalkMessage()
    {
        npcTextTMP.text = "Không thể nói chuyện!";
        npcText.SetActive(true);
        yield return new WaitForSeconds(2f);
        npcText.SetActive(false);
    }

}
