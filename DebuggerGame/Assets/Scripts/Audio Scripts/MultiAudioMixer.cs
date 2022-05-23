using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Board))]
public class MultiAudioMixer : MonoBehaviour
{
    [SerializeField]
    LevelMusicDescription levelMusic;

    [SerializeField]
    float transitionSpeed = 1f;

    AudioSource glitchSource, normalSource;

    float glitchVolume, normalVolume;

    private Board board;

    private int nBugsTotal;

    private bool ready = false;

    void Start()
    {
        board = GetComponent<Board>();
        board.ReadyEvent.AddListener(OnReady);
        board.BugsCaughtChangeEvent.AddListener(OnBugsCaughtChange);
    }

    void OnReady() {
        nBugsTotal = board.numBugs;

        glitchSource = gameObject.AddComponent<AudioSource>();
        normalSource = gameObject.AddComponent<AudioSource>();

        glitchSource.loop = true;
        normalSource.loop = true;

        glitchSource.clip = levelMusic.glitchClipInfo.clip;
        normalSource.clip = levelMusic.normalClipInfo.clip;

        if (levelMusic.glitchClipInfo.mixerGroup != null)
            glitchSource.outputAudioMixerGroup = levelMusic.glitchClipInfo.mixerGroup;
        if (levelMusic.normalClipInfo.mixerGroup != null)
            glitchSource.outputAudioMixerGroup = levelMusic.normalClipInfo.mixerGroup;

        UpdateVolumes();

        glitchSource.volume = glitchVolume;
        normalSource.volume = normalVolume;

        glitchSource.Play();
        normalSource.Play();

        ready = true;
    }

    void OnBugsCaughtChange() {
        UpdateVolumes();
    }

    void UpdateVolumes()
    {
        glitchVolume = 
            (levelMusic.glitchClipInfo.maxVolume - levelMusic.glitchClipInfo.minVolume)
            * (nBugsTotal - board.nBugsCaught) / nBugsTotal;

        normalVolume = 
            (levelMusic.normalClipInfo.maxVolume - levelMusic.normalClipInfo.minVolume)
            * board.nBugsCaught / nBugsTotal;

        //bool success;

        /*float volume = (nBugsTotal - board.numBugs) == 0
            ? zeroVolume
            : (maxVolume - minVolume) * (nBugsTotal - board.numBugs) / nBugsTotal + minVolume;

        success = mixer.SetFloat(NormalVolume, volume);
        Debug.AssertFormat(success, "Could not set NormalVolume");

        volume = board.numBugs == 0
            ? zeroVolume
            : (maxVolume - minVolume) * board.numBugs / nBugsTotal + minVolume;

        success = mixer.SetFloat(GlitchVolume, volume);
        Debug.AssertFormat(success, "Could not set GlitchVolume");*/
    }

    private void Update()
    {
        if (ready)
        {
            glitchSource.volume = Mathf.Lerp(glitchSource.volume, glitchVolume, Time.deltaTime * transitionSpeed);
            normalSource.volume = Mathf.Lerp(normalSource.volume, normalVolume, Time.deltaTime * transitionSpeed);
        }
    }

    private void OnDestroy()
    {
        board.ReadyEvent.RemoveListener(OnReady);
        board.BugsCaughtChangeEvent.RemoveListener(OnBugsCaughtChange);
    }
}
