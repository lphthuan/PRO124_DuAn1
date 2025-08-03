using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCText : MonoBehaviour
{
    public GameObject textObject;
    public AudioSource audioSource;
    private void Start()
    {
        audioSource.GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
            textObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            textObject.SetActive(false);
    }
    public void playAudio()
    {
        audioSource.Play();
    }

}