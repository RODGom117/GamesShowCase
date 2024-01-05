using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanequeSounds : MonoBehaviour
{
   
    private AudioSource audioSource;

    // List of audio clips for voicelines
    public AudioClip[] voicelinePlaylist;

    // Index to keep track of the current playing voiceline
    private int currentVoicelineIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Check if there are any voicelines in the playlist
        if (voicelinePlaylist.Length > 0)
        {
            // Play the first voiceline
            PlayNextVoiceline();
        }
        else
        {
            Debug.LogError("Voiceline playlist is empty. Add audio clips to the 'voicelinePlaylist' array.");
        }
    }

    // Call this method to play the next voiceline in the playlist
    private void PlayNextVoiceline()
    {
        if (voicelinePlaylist.Length > 0)
        {
            // Play a random voiceline
            int randomIndex = Random.Range(0, voicelinePlaylist.Length);
            audioSource.clip = voicelinePlaylist[randomIndex];
            audioSource.Play();

            // Move to the next voiceline index
            currentVoicelineIndex = randomIndex;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the current voiceline has finished playing
        if (!audioSource.isPlaying)
        {
            // Play the next voiceline when the current one finishes
            PlayNextVoiceline();
        }
    }
}
