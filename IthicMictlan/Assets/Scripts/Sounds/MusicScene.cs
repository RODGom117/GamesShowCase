using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScene : MonoBehaviour
{
    private AudioSource audioSource;

    // List of audio clips for music
    public AudioClip[] musicList;

    // AudioClip for the final boss song
    public AudioClip finalBossClip;

    // Index to keep track of the current playing song
    private int currentSongIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Check if there are any songs in the list
        if (musicList.Length > 0)
        {
            // Play the first song
            PlayNextSong();
        }
        else
        {
            Debug.LogError("Music list is empty. Add audio clips to the 'musicList' array.");
        }
    }

    // Call this method to play the next song in the list
    public void PlayNextSong()
    {
        if (musicList.Length > 0)
        {
            // Play the current song
            audioSource.clip = musicList[currentSongIndex];
            audioSource.Play();

            // Move to the next song index
            currentSongIndex = (currentSongIndex + 1) % musicList.Length;
        }
    }

    // Call this method to stop any playing music and start the final boss song
    public void PlayFinalBossSong()
    {
        // Stop the currently playing music
        audioSource.Stop();

        // Play the final boss song
        audioSource.clip = finalBossClip;
        audioSource.loop = true;  // Set the loop property to true
        audioSource.Play();
    }
}