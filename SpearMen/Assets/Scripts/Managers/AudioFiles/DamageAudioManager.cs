using UnityEngine;

public class DamageAudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Damage Sounds")]
    public AudioClip[] damageClips;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayDamageSFX()
    {
        if (audioSource == null || damageClips == null || damageClips.Length == 0)
            return;

        int index = Random.Range(0, damageClips.Length);
        audioSource.PlayOneShot(damageClips[index]);
    }
}
