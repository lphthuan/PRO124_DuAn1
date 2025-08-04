using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemorySequenceManager : MonoBehaviour
{
    [Header("Thư mục emoji trong Resources/")]
    public string emojiFolderName = "EmojiSprites"; // Thư mục: Assets/Resources/EmojiSprites

    [Header("Hiển thị chuỗi lên UI Panel")]
    public Image[] sequenceSlots; // Kéo các Image UI để hiển thị emoji gợi nhớ

    [Header("Số lượng emoji cần nhớ")]
    [Range(1, 64)] public int sequenceLengthInInspector = 10;

    [HideInInspector] public List<Sprite> emojiLibrary = new List<Sprite>();
    [HideInInspector] public List<int> targetSequence = new List<int>();

    [Header("Các emoji target trong scene")]
    public List<EmojiTarget> emojiTargets = new List<EmojiTarget>();

    private int currentIndex = 0;

    [Header("Giao diện đếm thời gian ghi nhớ")]
    public TMP_Text countdownText; // Gán TMP_Text trong Inspector
    public float memorizationTime = 10f; // Thời gian người chơi ghi nhớ

    public GameObject Telezone;

    private void Awake()
    {
        LoadEmojiLibraryFromFolder();
        GenerateSequence();
        ShowSequence();
    }

    private void Start()
    {
        StartCoroutine(MemorizationCountdown()); // Bắt đầu đếm ngược khi hiện chuỗi
    }

    private void LoadEmojiLibraryFromFolder()
    {
        emojiLibrary.Clear();
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>(emojiFolderName);
        emojiLibrary.AddRange(loadedSprites);
    }

    public void GenerateSequence()
    {
        targetSequence.Clear();
        for (int i = 0; i < sequenceLengthInInspector; i++)
        {
            int randomIndex = Random.Range(0, emojiLibrary.Count);
            targetSequence.Add(randomIndex);
        }

        currentIndex = 0;
    }

    public void ShowSequence()
    {
        for (int i = 0; i < sequenceSlots.Length; i++)
        {
            if (i < targetSequence.Count)
            {
                sequenceSlots[i].sprite = emojiLibrary[targetSequence[i]];
                sequenceSlots[i].enabled = true;
            }
            else
            {
                sequenceSlots[i].enabled = false;
            }
        }
    }

    public void OnEmojiSelected(int selectedIndex)
    {
        if (selectedIndex == targetSequence[currentIndex])
        {
            currentIndex++;

            if (currentIndex >= targetSequence.Count)
            {
                Debug.Log("Hoàn thành chuỗi chính xác!");
                // Xử lý thắng puzzle
            }
        }
        else
        {
            currentIndex = 0;
        }
    }

    public int GetCorrectEmojiIndexForTarget(EmojiTarget target)
    {
        int index = emojiTargets.IndexOf(target);
        if (index >= 0 && index < targetSequence.Count)
            return targetSequence[index];

        Debug.LogWarning("Không tìm thấy EmojiTarget tương ứng hoặc index nằm ngoài giới hạn!");
        return -1;
    }

    private IEnumerator MemorizationCountdown()
    {
        countdownText.gameObject.SetActive(true);
        float remainingTime = memorizationTime;

        while (remainingTime > 0f)
        {
            countdownText.text = $"Ghi nhớ: {Mathf.CeilToInt(remainingTime)}s";
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        // Khi hết giờ:
        countdownText.text = "Đã hết thời gian ghi nhớ!";

        yield return new WaitForSeconds(1f); // Cho người chơi thấy thông báo một chút

        countdownText.gameObject.SetActive(false);
        gameObject.SetActive(false); // Tắt toàn bộ MemorySequenceManager
        Telezone.gameObject.SetActive(false);
    }

}
