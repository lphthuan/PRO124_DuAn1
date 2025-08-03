using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BlacksmithDialogue : MonoBehaviour
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
    public Button upgradeHealthButton;
    public Button upgradeSpeedButton;
    public AudioSource backgroundMusic;

    [Header("Floating Text")]
    public GameObject floatingTextPrefab;

    [Header("Upgrade Souls")]
    public int damageUpgradeSoul = 3;
    public int healthUpgradeSoul = 3;
    public int speedUpgradeSoul = 2;

    [Header("Upgrade Soul Texts")]
    public TMP_Text damageSoulText;
    public TMP_Text healthSoulText;
    public TMP_Text speedSoulText;

    private int currentLine = 0;
    private bool playerInRange = false;
    private bool isTalking = false;
    private bool isTyping = false;

    private int damageUpgradeCount;
    private int healthUpgradeCount;
    private int speedUpgradeCount;

    private const int maxDamageUpgradeCount = 5;
    private const int maxHealthUpgradeCount = 10;
    private const int maxSpeedUpgradeCount = 5;

    private GameObject player;
    private MonoBehaviour[] playerScriptsToDisable;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        npcText.SetActive(false);
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        shopPanel.SetActive(false);

        // Gỡ mọi listener trước khi gán
        upgradeDamageButton.onClick.RemoveAllListeners();
        upgradeHealthButton.onClick.RemoveAllListeners();
        upgradeSpeedButton.onClick.RemoveAllListeners();
        buyButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        shopExitButton.onClick.RemoveAllListeners();

        // Gán mới
        upgradeDamageButton.onClick.AddListener(UpgradePlayerDamage);
        upgradeHealthButton.onClick.AddListener(UpgradePlayerHealth);
        upgradeSpeedButton.onClick.AddListener(UpgradePlayerSpeed);
        buyButton.onClick.AddListener(OpenShop);
        exitButton.onClick.AddListener(CloseDialogue);
        shopExitButton.onClick.AddListener(CloseShopToChoices);

        // Gán hiển thị giá
        damageSoulText.text = $"Soul: {damageUpgradeSoul}";
        healthSoulText.text = $"Soul: {healthUpgradeSoul}";
        speedSoulText.text = $"Soul: {speedUpgradeSoul}";

        damageUpgradeCount = PlayerPrefs.GetInt("UpgradeDamageCount", 0);
        healthUpgradeCount = PlayerPrefs.GetInt("UpgradeHealthCount", 0);
        speedUpgradeCount = PlayerPrefs.GetInt("UpgradeSpeedCount", 0);
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
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
        currentLine++;
    }

    private void EndDialogue()
    {
        choicePanel.SetActive(true);
    }

    private void OpenShop()
    {
        shopPanel.SetActive(true);
        choicePanel.SetActive(false);
        dialoguePanel.SetActive(false);

        UpdateShopCostColors();
    }

    private void UpgradePlayerDamage()
    {
        if (damageUpgradeCount >= maxDamageUpgradeCount)
        {
            ShowFloatingText("Đã đạt giới hạn nâng cấp!");
            return;
        }

        if (!SoulUIManager.instance.SpendSoul(damageUpgradeSoul))
        {
            ShowFloatingText("Không đủ Soul!");
            return;
        }

        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            playerAttack.baseDamage += 20f;
            damageUpgradeCount++;
            PlayerPrefs.SetFloat("PlayerDamage", playerAttack.baseDamage);
            PlayerPrefs.SetInt("UpgradeDamageCount", damageUpgradeCount);
            PlayerPrefs.Save();
            ShowFloatingText($"Damage +20 ({damageUpgradeCount}/{maxDamageUpgradeCount})");
        }
    }

    private void UpgradePlayerHealth()
    {
        if (healthUpgradeCount >= maxHealthUpgradeCount)
        {
            ShowFloatingText("Đã đạt giới hạn nâng cấp!");
            return;
        }

        if (!SoulUIManager.instance.SpendSoul(healthUpgradeSoul))
        {
            ShowFloatingText("Không đủ Soul!");
            return;
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.maxHealth += 100;
            healthUpgradeCount++;
            PlayerPrefs.SetInt("PlayerHealth", playerHealth.maxHealth);
            PlayerPrefs.SetInt("UpgradeHealthCount", healthUpgradeCount);
            PlayerPrefs.Save();
            ShowFloatingText($"Health +100 ({healthUpgradeCount}/{maxHealthUpgradeCount})");
        }
    }


    private void UpgradePlayerSpeed()
    {
        if (speedUpgradeCount >= maxSpeedUpgradeCount)
        {
            ShowFloatingText("Đã đạt giới hạn nâng cấp!");
            return;
        }

        if (!SoulUIManager.instance.SpendSoul(speedUpgradeSoul))
        {
            ShowFloatingText("Không đủ Soul!");
            return;
        }

        PlayerController move = player.GetComponent<PlayerController>();
        if (move != null)
        {
            move.moveSpeed += 1;
            speedUpgradeCount++;
            PlayerPrefs.SetFloat("PlayerSpeed", move.moveSpeed);
            PlayerPrefs.SetInt("UpgradeSpeedCount", speedUpgradeCount);
            PlayerPrefs.Save();
            ShowFloatingText($"Speed +1 ({speedUpgradeCount}/{maxSpeedUpgradeCount})");
        }
    }
    private void UpdateShopCostColors()
    {
        int currentSoul = SoulUIManager.instance.GetCurrentSoul();

        damageSoulText.color = (currentSoul >= damageUpgradeSoul) ? Color.white : Color.red;
        healthSoulText.color = (currentSoul >= healthUpgradeSoul) ? Color.white : Color.red;
        speedSoulText.color = (currentSoul >= speedUpgradeSoul  ) ? Color.white : Color.red;
    }


    private void ShowFloatingText(string message)
    {
        if (floatingTextPrefab != null && player != null)
        {
            Vector3 spawnPos = player.transform.position + new Vector3(0, 1.5f, 0);
            GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);

            FloatingTextController ft = textObj.GetComponent<FloatingTextController>();
            if (ft != null)
            {
                ft.ShowText(message);
            }

            Destroy(textObj, 2f);
        }
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
        }
    }

    private void CloseShopToChoices()
    {
        shopPanel.SetActive(false);
        choicePanel.SetActive(true);
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

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("PlayerDamage");
        PlayerPrefs.DeleteKey("UpgradeDamageCount");
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("UpgradeHealthCount");
        PlayerPrefs.DeleteKey("PlayerSpeed");
        PlayerPrefs.DeleteKey("UpgradeSpeedCount");

        Debug.Log("Đã xóa toàn bộ dữ liệu nâng cấp.");
    }
}