using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemorySequenceManager : MonoBehaviour
{
    public string emojiFolderName = "EmojiSprites";
    public Image[] sequenceSlots;
    [Range(1, 64)] public int sequenceLengthInInspector = 10;

    [HideInInspector] public List<Sprite> emojiLibrary = new List<Sprite>();
    [HideInInspector] public List<Sprite> targetSequence = new List<Sprite>();

    public List<EmojiTarget> emojiTargets = new List<EmojiTarget>();
    private Sprite[] currentSelectedSprites;

    public TMP_Text countdownText;
    public float memorizationTime = 10f;
    public GameObject Telezone4B;
    public GameObject Telezone4C;

    private void Awake()
    {
        LoadEmojiLibraryFromFolder();
        GenerateSequence();
        SetupTargets();
        ShowSequence();
    }

    private void Start()
    {
        Telezone4C.SetActive(false);
    }

    public void StartMemorization()
    {
        StopAllCoroutines();
        if (countdownText != null) countdownText.gameObject.SetActive(true);
        StartCoroutine(MemorizationCountdown());
    }

    private void LoadEmojiLibraryFromFolder()
    {
        emojiLibrary.Clear();
        Sprite[] loaded = Resources.LoadAll<Sprite>(emojiFolderName);
        emojiLibrary.AddRange(loaded);
        Debug.Log($"Loaded {emojiLibrary.Count} sprites from {emojiFolderName}");
    }

    public void GenerateSequence()
    {
        targetSequence.Clear();
        if (emojiLibrary.Count == 0) return;

        for (int i = 0; i < sequenceLengthInInspector; i++)
        {
            int r = Random.Range(0, emojiLibrary.Count);
            targetSequence.Add(emojiLibrary[r]);
        }
        currentSelectedSprites = new Sprite[targetSequence.Count];
        for (int i = 0; i < currentSelectedSprites.Length; i++) currentSelectedSprites[i] = null;
    }

    private void SetupTargets()
    {
        // Bắt buộc: thứ tự trong emojiTargets phải tương ứng với thứ tự sequenceSlots (slot 0 -> emojiTargets[0])
        for (int i = 0; i < emojiTargets.Count; i++)
        {
            int slot = (i < targetSequence.Count) ? i : -1;
            emojiTargets[i].Initialize(this, slot);
        }
    }

    public void ShowSequence()
    {
        for (int i = 0; i < sequenceSlots.Length; i++)
        {
            if (i < targetSequence.Count)
            {
                sequenceSlots[i].sprite = targetSequence[i];
                sequenceSlots[i].enabled = true;
            }
            else sequenceSlots[i].enabled = false;
        }
    }

    // Gọi khi 1 target thay đổi; slotIndex được gán khi Initialize
    public void OnTargetChanged(int slotIndex, Sprite selectedSprite)
    {
        if (slotIndex < 0 || slotIndex >= targetSequence.Count) return;

        currentSelectedSprites[slotIndex] = selectedSprite;
        Debug.Log($"Slot {slotIndex} changed -> {selectedSprite?.name}. Expect {targetSequence[slotIndex].name}");

        // Tự động lock nếu đúng
        if (selectedSprite == targetSequence[slotIndex])
            emojiTargets[slotIndex].LockEmoji();
        else
            emojiTargets[slotIndex].UnlockEmoji();

        if (CheckAllMatched())
            OnSequenceCompleted();
    }

    private bool CheckAllMatched()
    {
        for (int i = 0; i < targetSequence.Count; i++)
        {
            if (currentSelectedSprites[i] != targetSequence[i]) return false;
        }
        return true;
    }

    private void OnSequenceCompleted()
    {
        Debug.Log("Hoàn thành chuỗi chính xác!");
        if (Telezone4C != null) Telezone4C.SetActive(true);
        foreach (var t in emojiTargets) t.LockEmoji();
    }

    public Sprite GetCorrectSpriteForSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < targetSequence.Count) return targetSequence[slotIndex];
        return null;
    }

    private IEnumerator MemorizationCountdown()
    {
        if (countdownText != null) countdownText.gameObject.SetActive(true);
        float remaining = memorizationTime;
        while (remaining > 0f)
        {
            if (countdownText != null) countdownText.text = $"Ghi nhớ: {Mathf.CeilToInt(remaining)}s";
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }
        if (countdownText != null) countdownText.text = "Đã hết thời gian ghi nhớ!";
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        if (Telezone4B != null) Telezone4B.SetActive(false);
    }
}
