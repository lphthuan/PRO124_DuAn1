using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBoss : MonoBehaviour
{
    [Header("Âm thanh")]
    public AudioClip soundClip;
    public float volume = 1f;

    private bool hasPlayed = false;

    private AudioSource audioSource;

    private void Awake()
    {
        // Gắn AudioSource sẵn hoặc tự thêm 1 lần duy nhất
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.clip = soundClip;
        audioSource.volume = volume;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasPlayed) return;

        if (other.CompareTag("Player"))
        {
            hasPlayed = true;
            audioSource.Play();

            // Tắt collider nhưng không destroy ngay lập tức
            GetComponent<Collider2D>().enabled = false;

            // Xoá object sau khi nhạc phát xong
            Destroy(gameObject, soundClip.length);
        }
    }
}

