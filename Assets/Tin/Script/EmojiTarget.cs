using UnityEngine;

public class EmojiTarget : MonoBehaviour
{
    public SpriteRenderer emojiRenderer;
    public int maxChangeCount = 8;

    private MemorySequenceManager manager;
    private int slotIndex = -1;
    private int currentEmojiIndex = 0;
    private int changeCount = 0;
    private bool isLocked = false;

    public void Initialize(MemorySequenceManager mgr, int slot)
    {
        manager = mgr;
        slotIndex = slot;
    }

    private void Start()
    {
        if (manager == null || manager.emojiLibrary.Count == 0)
        {
            Debug.LogError("Manager chưa gán hoặc emojiLibrary rỗng!");
            return;
        }

        currentEmojiIndex = Random.Range(0, manager.emojiLibrary.Count);
        emojiRenderer.sprite = manager.emojiLibrary[currentEmojiIndex];

        // Báo trạng thái ban đầu cho manager
        if (slotIndex >= 0)
            manager.OnTargetChanged(slotIndex, emojiRenderer.sprite);
    }

    public void OnHitByWindSpell()
    {
        if (isLocked) return;
        if (changeCount >= maxChangeCount)
        {
            Debug.Log("Đã hết lượt thay đổi emoji.");
            return;
        }
        changeCount++;

        Sprite correct = manager.GetCorrectSpriteForSlot(slotIndex);
        float chance = Mathf.Clamp01((float)changeCount / maxChangeCount);

        if (correct != null && Random.value < chance)
        {
            // đặt đúng
            // tìm index của correct trong manager.emojiLibrary để set currentEmojiIndex (nếu cần)
            currentEmojiIndex = manager.emojiLibrary.IndexOf(correct);
            if (currentEmojiIndex < 0) currentEmojiIndex = 0;
        }
        else
        {
            currentEmojiIndex = (currentEmojiIndex + 1) % manager.emojiLibrary.Count;
        }

        emojiRenderer.sprite = manager.emojiLibrary[currentEmojiIndex];
        if (slotIndex >= 0)
            manager.OnTargetChanged(slotIndex, emojiRenderer.sprite);
    }

    public void ChangeEmoji()
    {
        if (isLocked) return;
        currentEmojiIndex = (currentEmojiIndex + 1) % manager.emojiLibrary.Count;
        emojiRenderer.sprite = manager.emojiLibrary[currentEmojiIndex];
        if (slotIndex >= 0)
            manager.OnTargetChanged(slotIndex, emojiRenderer.sprite);
    }

    public void LockEmoji() => isLocked = true;
    public void UnlockEmoji() => isLocked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WindSpell")) OnHitByWindSpell();
    }
}
