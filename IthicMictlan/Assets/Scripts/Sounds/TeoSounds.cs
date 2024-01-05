using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeoSounds : MonoBehaviour
{
    
        private AudioSource audioSource;

    // Lists for different types of voice lines
    public AudioClip[] hability1VoiceLines;
    public AudioClip[] hability2VoiceLines;
    public AudioClip[] ultimateVoiceLines;
    public AudioClip[] defeatVoiceLines;
    public AudioClip[] hitReceivedVoiceLines;
    public AudioClip[] hitVoiceLines;
    public AudioClip[] basicAttackVoiceLines;
    public AudioClip[] finalBossVoiceLines;
    public AudioClip[] selectionVoiceLines;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // Call this method with the index of the voice line you want to play for Hability 1
    public void PlayHability1Voice()
    {
        PlayRandomVoice(hability1VoiceLines);
    }

    // Call this method with the index of the voice line you want to play for Hability 2
    public void PlayHability2Voice()
    {
        PlayRandomVoice(hability2VoiceLines);
    }

    // Call this method with the index of the voice line you want to play for Ultimate
    public void PlayUltimateVoice()
    {
        PlayRandomVoice(ultimateVoiceLines);
    }

    // Call this method with the index of the voice line you want to play for Defeat
    public void PlayDefeatVoice()
    {
        PlayRandomVoice(defeatVoiceLines);
    }

    // Call this method with the index of the voice line you want to play for Hit Received
    public void PlayHitReceivedVoice()
    {
        PlayRandomVoice(hitReceivedVoiceLines);
    }

    // Call this method with the index of the voice line you want to play for Hit
    public void PlayHitVoice()
    {
        PlayRandomVoice(hitVoiceLines);
    }

    // Call this method with the index of the voice line you want to play for Basic Attack
    public void PlayBasicAttackVoice()
    {
        PlayRandomVoice(basicAttackVoiceLines);
    }

    // Call this method with the index of the voice line you want to play for Final Boss
    public void PlayFinalBossVoice()
    {
        PlayRandomVoice(finalBossVoiceLines);
    }

    // Call this method with the index of the voice line you want to play for Selection
    public void PlaySelectionVoice()
    {
        PlayRandomVoice(selectionVoiceLines);
    }

    // Private helper method to play a voice line from a given list
    private void PlayVoice(AudioClip[] voiceLineList, int voiceLineIndex)
    {
        if (!audioSource.isPlaying && voiceLineIndex >= 0 && voiceLineIndex < voiceLineList.Length)
        {
            audioSource.clip = voiceLineList[voiceLineIndex];
            audioSource.Play();
        }
    }

     private void PlayRandomVoice(AudioClip[] voiceLineList)
    {
        if (voiceLineList.Length > 0)
        {
            int randomIndex = Random.Range(0, voiceLineList.Length);
            audioSource.clip = voiceLineList[randomIndex];
            audioSource.Play();
        }
    }
}