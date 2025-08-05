using UnityEngine;

public class EmojiTarget : MonoBehaviour
{
    public SpriteRenderer emojiRenderer;
    public MemorySequenceManager manager;
    public int maxChangeCount = 8;

    private int currentEmojiIndex = 0;
    private int changeCount = 0;

    private bool isLocked = false;

    private int correctEmojiIndex = -1;

    private void Start()
    {
        if (manager == null || manager.emojiLibrary == null || manager.emojiLibrary.Count == 0)
        {
            Debug.LogError("Manager hoặc emojiLibrary chưa được gán!");
            return;
        }

        correctEmojiIndex = manager.GetCorrectEmojiIndexForTarget(this);
        currentEmojiIndex = Random.Range(0, manager.emojiLibrary.Count);
        emojiRenderer.sprite = manager.emojiLibrary[currentEmojiIndex];
    }

    public void OnHitByWindSpell()
    {
        if (changeCount >= maxChangeCount)
        {
            Debug.Log("🛑 Đã hết lượt thay đổi emoji.");
            return;
        }

        changeCount++;

        int correctEmojiIndex = manager.GetCorrectEmojiIndexForTarget(this);
        if (correctEmojiIndex == -1)
        {
            Debug.LogWarning("❌ Không thể xác định emoji đúng cho target này.");
            return;
        }

        // Tăng xác suất ra emoji đúng nếu sắp hết lượt
        float chance = Mathf.Clamp01((float)changeCount / maxChangeCount);
        if (Random.value < chance)
        {
            currentEmojiIndex = correctEmojiIndex;
        }
        else
        {
            currentEmojiIndex++;
            if (currentEmojiIndex >= manager.emojiLibrary.Count)
                currentEmojiIndex = 0;
        }

        emojiRenderer.sprite = manager.emojiLibrary[currentEmojiIndex];
        manager.OnEmojiSelected(currentEmojiIndex);
    }

    public void ChangeEmoji()
    {
        if (isLocked) return;

        currentEmojiIndex = (currentEmojiIndex + 1) % manager.emojiLibrary.Count;
        emojiRenderer.sprite = manager.emojiLibrary[currentEmojiIndex];

        manager.OnEmojiSelected(currentEmojiIndex);
    }

    public void LockEmoji()
    {
        isLocked = true;
    }

    public void UnlockEmoji()
    {
        isLocked = false;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WindSpell"))
        {
            OnHitByWindSpell();
        }
    }
}