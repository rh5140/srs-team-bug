using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Music", menuName = "Music/Level Music Description", order = 1)]
public class LevelMusicDescription : ScriptableObject
{
    [Serializable]
    public class AudioClipInfo
    {
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        public float maxVolume = 1f;
        public float minVolume;
    }

    public AudioClipInfo glitchClipInfo, normalClipInfo;
}
