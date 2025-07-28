using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBoss : MonoBehaviour
{
    [Header("Âm thanh")]
    public AudioClip soundClip;
    public float volume = 1f;

    private bool hasPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasPlayed) return;

        if (other.CompareTag("Player"))
        {
            hasPlayed = true;

            // Tạo một GameObject tạm thời chỉ để phát nhạc
            GameObject soundObject = new GameObject("OneShotAudio");
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = soundClip;
            audioSource.volume = volume;
            audioSource.Play();

            // Xoá object sau khi nhạc kết thúc
            Destroy(soundObject, soundClip.length);

            // Xoá collider này (object hiện tại)
            Destroy(gameObject);
        }
    }
}
