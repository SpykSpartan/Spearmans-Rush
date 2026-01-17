using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeMixer : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(value) * 20); 
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(value) * 20);
    }

    public void SetDialogueVolume(float value)
    {
        audioMixer.SetFloat("DialogueVol", Mathf.Log10(value) * 20);
    }

    public void SetUIVolume(float value)
    {
        audioMixer.SetFloat("UIVol", Mathf.Log10(value) * 20);
    }
}
