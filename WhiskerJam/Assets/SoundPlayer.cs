using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip applause;
    [SerializeField] private AudioClip meows;

    private void Start()
    {
        audioSource.PlayOneShot(applause);
        audioSource.PlayOneShot(meows);
    }
}
