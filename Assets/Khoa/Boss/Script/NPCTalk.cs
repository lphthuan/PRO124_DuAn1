using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCTalk : MonoBehaviour
{
    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public GameObject npcText;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public string[] dialogueLines;
    public float typingSpeed = 0.05f;

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool isTalking = false;
    private bool isTyping = false;

    private GameObject player;
    private MonoBehaviour[] playerScriptsToDisable;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        npcText.SetActive(false);
        dialoguePanel.SetActive(false);
    }


    private void Update()
    {
        if (playerInRange && !isTalking && Input.GetKeyDown(KeyCode.E))
        {
            npcText.SetActive(false);
            StartDialogue();

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
            CloseDialogue();
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


    private void OpenShop()
    {
        dialoguePanel.SetActive(false);

    }

    

    private void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        isTalking = false;

        EnablePlayerControl();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            npcText.SetActive(true);
            playerScriptsToDisable = player.GetComponents<MonoBehaviour>();
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

    private void CloseShopToChoices()
    {
        dialoguePanel.SetActive(true);
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
}
