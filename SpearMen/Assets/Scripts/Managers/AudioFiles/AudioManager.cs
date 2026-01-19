using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("SFX Pool")]
    public int poolSize = 8;
    private AudioSource[] sfxPool;
    private int poolIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        sfxPool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            sfxPool[i] = src;
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        poolIndex = (poolIndex + 1) % sfxPool.Length;
        sfxPool[poolIndex].PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
}
