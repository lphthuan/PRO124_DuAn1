using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class physician: MonoBehaviour
{
    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public GameObject npcText;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public string[] dialogueLines;
    public float typingSpeed = 0.05f;

    [Header("Choices")]
    public GameObject choicePanel;
    public Button buyButton;
    public Button exitButton;

    [Header("Shop")]
    public GameObject shopPanel;
    public Button shopExitButton;
    public Button upgradeDamageButton;
    public AudioSource backgroundMusic;

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool isTalking = false;
    private bool isTyping = false;

    private GameObject player;
    private MonoBehaviour[] playerScriptsToDisable;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        shopPanel.SetActive(false);

        upgradeDamageButton.onClick.AddListener(UpgradePlayerDamage);

        buyButton.onClick.AddListener(OpenShop);
        exitButton.onClick.AddListener(CloseDialogue);
        shopExitButton.onClick.AddListener(CloseShopToChoices);
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

        if (backgroundMusic != null)
        {
            backgroundMusic.Pause();
        }

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
        else
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
            yield return new WaitForSecondsRealtime(typingSpeed); // dùng Realtime để không bị Time.timeScale ảnh hưởng
        }
        isTyping = false;
        currentLine++;
    }

    private void EndDialogue()
    {
        choicePanel.SetActive(true);
        // dialoguePanel vẫn giữ nguyên
    }

    private void OpenShop()
    {
        shopPanel.SetActive(true);
        choicePanel.SetActive(false);
        dialoguePanel.SetActive(true);
    }


    private void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        shopPanel.SetActive(false);
        isTalking = false;

        if (backgroundMusic != null)
        {
            backgroundMusic.UnPause();
        }

        EnablePlayerControl(); // bật lại các script
    }

    private void UpgradePlayerDamage()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
            if (playerAttack != null)
            {
                playerAttack.baseDamage += 10f; // tăng 10 damage

                // Lưu lại vĩnh viễn
                PlayerPrefs.SetFloat("PlayerDamage", playerAttack.baseDamage);
                PlayerPrefs.Save();

                Debug.Log("Đã tăng sát thương lên: " + playerAttack.baseDamage);
            }
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("PlayerDamage"); // Xoá khi game tắt

        Debug.Log("Đã xóa Key: PlayerDamage");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;

            // Lưu các script cần disable
            playerScriptsToDisable = player.GetComponents<MonoBehaviour>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    private void CloseShopToChoices()
    {
        shopPanel.SetActive(false);       // Tắt panel shop
        choicePanel.SetActive(true);      // Hiện lại panel lựa chọn
        dialoguePanel.SetActive(true);    // Nếu bạn muốn vẫn hiển thị hội thoại ở dưới
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
