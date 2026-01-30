using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    // Singleton Instance 
    public static SoundManager Instance { get; private set; }


    [Header("Audio Sources")]
    // We use two sources so we can mix have different vol values
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;


    [Header("Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    // Dictionary
    private Dictionary<string, SoundData> _soundDictionary;


    #region Initialization
    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Load all SoundData from resources
        SoundData[] allSounds = Resources.LoadAll<SoundData>("Audio");

        //Initialize the sound diccionary
        _soundDictionary = new Dictionary<string, SoundData>();
        foreach (var sound in allSounds)
        {
            if (sound == null || string.IsNullOrEmpty(sound.soundName)) continue;

            if (!_soundDictionary.ContainsKey(sound.soundName))
                _soundDictionary.Add(sound.soundName, sound);
            else
                Debug.LogWarning($"Sound '{sound.soundName}' is duplicated in SoundManager.");
        }

        // Set up global sounds settings
        _musicSource.spatialBlend = 0.0f;
        _sfxSource.spatialBlend = 0.0f;
    }

    private void Start()
    {
        // Get vols from player prefs
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }
    #endregion

    #region Public API

    // Play by string name
    public void Play(string soundName)
    {
        // If name is not found, give a log warning
        if (!_soundDictionary.TryGetValue(soundName, out SoundData data))
        {
            Debug.LogWarning($"Sound '{soundName}' not found!");
            return;
        }

        Play(data);
    }

    // Play by SoundData reference
    public void Play(SoundData data)
    {
        if (data == null) return;

        switch (data.soundType)
        {
            case SoundType.Music:
                PlayMusic(data.audioClip, data.volume);
                break;

            case SoundType.SFX:
                PlaySFX(data.audioClip, data.volume);
                break;
        }
    }
    #endregion

    #region Play and stop clips
    public void PlayMusic(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (clip == null) return;

        // If this music is already playing, do nothing
        if (_musicSource.clip == clip && _musicSource.isPlaying)
            return;

        _musicSource.clip = clip;
        _musicSource.volume = volume;
        _musicSource.loop = loop;
        _musicSource.Play();
    }

    public void PlayMusic(string musicName, bool loop = true)
    {
        if (string.IsNullOrEmpty(musicName)) return;

        if (!_soundDictionary.TryGetValue(musicName, out SoundData data))
        {
            Debug.LogWarning($"Music '{musicName}' not found!");
            return;
        }

        if (data.soundType != SoundType.Music)
        {
            Debug.LogWarning($"Sound '{musicName}' is not marked as Music.");
            return;
        }

        PlayMusic(data.audioClip, data.volume, loop);
    }


    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip, volume);
    }

    #endregion
    
    #region Volume setters

    public static float VolumeToDecibels(float volume)
    {
        return volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20f;
    }

    public void SetMasterVolume(float volume)
    {
        float dB = VolumeToDecibels(volume);
        _audioMixer.SetFloat("MasterVolume", dB);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        float dB = VolumeToDecibels(volume);
        _audioMixer.SetFloat("MusicVolume", dB);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        float dB = VolumeToDecibels(volume);
        _audioMixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }


    #endregion
}