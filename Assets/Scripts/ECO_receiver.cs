using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System;

public class ECO_receiver : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip streamedClip;

    public bool debugSampleData;
    private float[] sampleDataArray;
    public float sampleSize;
    public float sampleFreq;

    private int outputSampleRate;
    public bool _bufferReady;

    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        sampleSize = 44100;
        sampleFreq = 44100;
        sampleDataArray = new float[2048];
        streamedClip = AudioClip.Create("audiostream", (int)sampleSize*2, 1, (int)sampleFreq, true, awesome);
        audioSource.clip = streamedClip;
        audioSource.Play();
        _bufferReady = true;

        
    }

    private void awesome(float[] data)
    {
        if (streamedClip)
        {
            streamedClip.SetData(data, 0);
            _bufferReady = false;
        }

        //StartCoroutine(awesomeMainThread(data));
    }

    IEnumerator awesomeMainThread(float[] data)
    {
        if (streamedClip)
        {
            streamedClip.SetData(data, 0);
            _bufferReady = false;
        }

        yield break;
    }

    private void Update()
    {
        if (GameState.Instance._mainCharacter == null) return;

        FillAudioBuffer();



    }

    void FillAudioBuffer()
    {
        //Debug.Log("FillAudioBuffer");
        audioSource.GetOutputData(sampleDataArray, 0);
        streamedClip.SetData(sampleDataArray, 0);
        _bufferReady = false;

        //StartCoroutine(FillAudioBufferMainThread());
    }

    IEnumerator FillAudioBufferMainThread() {
        Debug.Log("FillAudioBuffer");
        audioSource.GetOutputData(sampleDataArray, 0);
        streamedClip.SetData(sampleDataArray, 0);
        _bufferReady = false;
        yield break;
    }
}

