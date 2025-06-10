using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;

    [Header("Pitch Randomization")]
    public bool randomizePitch = true;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Instance = this;
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        if (randomizePitch)
        {
            sfxSource.pitch = Random.Range(minPitch, maxPitch);
        }
        else
        {
            sfxSource.pitch = 1f;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlaySFX(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip selected = clips[Random.Range(0, clips.Length)];
        PlaySFX(selected);
    }

    public void Play3DSFX(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;

        float pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : 1f;

        GameObject tempGO = new GameObject("Temp3DSFX");
        tempGO.transform.position = position;
        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = sfxVolume;
        source.spatialBlend = 1f;
        source.pitch = pitch;
        source.Play();
        Destroy(tempGO, clip.length / pitch);
    }

    public void Play3DSFX(AudioClip[] clips, Vector3 position)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip selected = clips[Random.Range(0, clips.Length)];
        Play3DSFX(selected, position);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}
