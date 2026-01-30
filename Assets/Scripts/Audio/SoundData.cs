using System;
using UnityEngine;

public enum SoundType { Music, SFX }

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Objects/SoundData")]
public class SoundData : ScriptableObject
{
    public string soundName;
    public SoundType soundType;
    public AudioClip audioClip;
    [Range(0f, 1f)]
    public float volume = 1f;
}
