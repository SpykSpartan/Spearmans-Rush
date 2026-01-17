using UnityEngine;

public class PlayerAttackAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource sfxSource;

    [Header("Attack Sounds")]
    public AudioClip[] bashSounds;
    public AudioClip[] slashSounds;

    public void PlayBash()
    {
        if (bashSounds.Length == 0 || sfxSource == null) return;

        AudioClip clip = bashSounds[Random.Range(0, bashSounds.Length)];
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySlash()
    {
        if (slashSounds.Length == 0 || sfxSource == null) return;

        AudioClip clip = slashSounds[Random.Range(0, slashSounds.Length)];
        sfxSource.PlayOneShot(clip);
    }
}
