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

    [Header("References")]
    public MemorySequenceManager memoryManager; // gán trong Inspector (hoặc tìm runtime)

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool isTalking = false;
    private bool isTyping = false;
    private bool hasTalked = false;

    private GameObject player;
    private MonoBehaviour[] playerScripts; // all scripts trên player
    private bool[] originalEnabledStates; // lưu trạng thái ban đầu

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

            // Lấy và lưu tất cả MonoBehaviour trên player (chỉ khi chưa lưu)
            if (playerScripts == null || playerScripts.Length == 0)
            {
                playerScripts = player.GetComponents<MonoBehaviour>();
                originalEnabledStates = new bool[playerScripts.Length];
                for (int i = 0; i < playerScripts.Length; i++)
                {
                    originalEnabledStates[i] = playerScripts[i].enabled;
                }
            }

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
        if (playerScripts != null)
        {
            for (int i = 0; i < playerScripts.Length; i++)
            {
                // không tắt chính script này nếu attach trên cùng object NPC/this
                if (playerScripts[i] != null && playerScripts[i] != this)
                {
                    playerScripts[i].enabled = false;
                }
            }
        }
    }

    private void EnablePlayerControl()
    {
        if (playerScripts != null && originalEnabledStates != null)
        {
            for (int i = 0; i < playerScripts.Length; i++)
            {
                if (playerScripts[i] != null)
                {
                    // restore trạng thái ban đầu
                    playerScripts[i].enabled = originalEnabledStates[i];
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

        // Gọi manager để bắt đầu memorization (nếu có)
        if (memoryManager == null)
        {
            memoryManager = FindObjectOfType<MemorySequenceManager>();
        }

        if (memoryManager != null)
        {
            // Nếu memoryManager object đang bị inactive, bật trước khi gọi
            if (!memoryManager.gameObject.activeInHierarchy)
                memoryManager.gameObject.SetActive(true);

            // gọi phương thức public để bắt đầu đếm / hiển thị
            memoryManager.StartMemorization(); // hoặc StartMemory() tùy bạn đã đặt tên
        }
        else
        {
            Debug.LogWarning("Không tìm thấy MemorySequenceManager khi nhấn Play.");
        }

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
