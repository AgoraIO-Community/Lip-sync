using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AudioRenderer : MonoBehaviour
{
    #region Fields, Properties, and Inner Classes
    // constants for the wave file header
    private const int HEADER_SIZE = 44;
    private const short BITS_PER_SAMPLE = 16;
    private const int SAMPLE_RATE = 44100;

    // the number of audio channels in the output file
    private int channels = 2;

    // the audio stream instance
    private MemoryStream outputStream;
    private BinaryWriter outputWriter;

    // should this object be rendering to the output stream?
    public bool Rendering = false;

    private AudioSource audioSource;
    //public AudioClip streamedClip;

    private float[] sampleDataArray;


    public double bpm = 140.0F;
    public float gain = 0.5F;
    public int signatureHi = 4;
    public int signatureLo = 4;

    private double nextTick = 0.0F;
    private float amp = 0.0F;
    private float phase = 0.0F;
    private double sampleRate = 0.0F;
    private int accent;
    private bool running = false;

    private bool isUsingAudioSource = false;

    /// The status of a render
    public enum Status
    {
        UNKNOWN,
        SUCCESS,
        FAIL,
        ASYNC
    }

    /// The result of a render.
    public class Result
    {
        public Status State;
        public string Message;

        public Result(Status newState = Status.UNKNOWN, string newMessage = "")
        {
            this.State = newState;
            this.Message = newMessage;
        }
    }
    #endregion

    public AudioRenderer()
    {
        this.Clear();
    }

    // reset the renderer
    public void Clear()
    {
        this.outputStream = new MemoryStream();
        this.outputWriter = new BinaryWriter(outputStream);
    }

    public void StartRecording() {
        Debug.Log("start recording");

        Clear();

        Rendering = true;

        if (isUsingAudioSource)
            audioSource.Play();
    }

    void Start()
    {
        if (isUsingAudioSource)
            audioSource = GetComponent<AudioSource>();
        float sampleSize = 44100;

        sampleDataArray = new float[2048];
        //streamedClip = AudioClip.Create("audiostream", (int)sampleSize * 2, 1, (int)sampleSize, true, awesome);
        //audioSource.clip = streamedClip;
        //audioSource.loop = true;
        //audioSource.Play();

        accent = signatureHi;
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;
        nextTick = startTick * sampleRate;
        running = true;
    }

    private void awesome(float[] data)
    {
        if (this.Rendering)
        {
            Debug.Log("awesome");
            // store the number of channels we are rendering
            // store the data stream
            this.Write(data);
        }

    }

    /// Write a chunk of data to the output stream.
    public void Write(float[] audioData)
    {
        // Convert numeric audio data to bytes
        for (int i = 0; i < audioData.Length; i++)
        {
            // write the short to the stream
            this.outputWriter.Write((short)(audioData[i] * (float)(32767))); //int 16 max value
        }
    }

    // write the incoming audio to the output string
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (this.Rendering)
        {
            Debug.Log("OnAudioFilterRead");
            // store the number of channels we are rendering
            this.channels = channels;

            // store the data stream

            //playBipSound(ref data, channels);

            this.Write(data);
        }

        
    }

    void playBipSound(ref float[] data, int channels) {
        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        double sample = AudioSettings.dspTime * sampleRate;
        int dataLen = data.Length / channels;

        int n = 0;
        while (n < dataLen)
        {
            float x = gain * amp * Mathf.Sin(phase);
            int i = 0;
            while (i < channels)
            {
                data[n * channels + i] += x;
                i++;
            }
            while (sample + n >= nextTick)
            {
                nextTick += samplesPerTick;
                amp = 1.0F;
                if (++accent > signatureHi)
                {
                    accent = 1;
                    amp *= 2.0F;
                }
                Debug.Log("Tick: " + accent + "/" + signatureHi);
            }
            phase += amp * 0.3F;
            amp *= 0.993F;
            n++;
        }
    }


    #region File I/O
    public AudioRenderer.Result Save(string filename)
    {
        Result result = new AudioRenderer.Result();

        if (outputStream.Length > 0)
        {
            // add a header to the file so we can send it to the SoundPlayer
            this.AddHeader();

            // if a filename was passed in
            if (filename.Length > 0)
            {
                // Save to a file. Print a warning if overwriting a file.
                if (File.Exists(filename))
                    Debug.LogWarning("Overwriting " + filename + "...");

                // reset the stream pointer to the beginning of the stream
                outputStream.Position = 0;

                // write the stream to a file
                FileStream fs = File.OpenWrite(filename);

                this.outputStream.WriteTo(fs);

                fs.Close();

                // for debugging only
                Debug.Log("Finished saving to " + filename + ".");
            }

            result.State = Status.SUCCESS;
        }
        else
        {
            Debug.LogWarning("There is no audio data to save!");

            result.State = Status.FAIL;
            result.Message = "There is no audio data to save!";
        }

        Debug.Log("save result is " + result.State.ToString());

        return result;
    }

    /// This generates a simple header for a canonical wave file, 
    /// which is the simplest practical audio file format. It
    /// writes the header and the audio file to a new stream, then
    /// moves the reference to that stream.
    /// 
    /// See this page for details on canonical wave files: 
    /// http://www.lightlink.com/tjweber/StripWav/Canon.html
    private void AddHeader()
    {
        // reset the output stream
        outputStream.Position = 0;

        // calculate the number of samples in the data chunk
        long numberOfSamples = outputStream.Length / (BITS_PER_SAMPLE / 8);

        // create a new MemoryStream that will have both the audio data AND the header
        MemoryStream newOutputStream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(newOutputStream);

        writer.Write(0x46464952); // "RIFF" in ASCII

        // write the number of bytes in the entire file
        writer.Write((int)(HEADER_SIZE + (numberOfSamples * BITS_PER_SAMPLE * channels / 8)) - 8);

        writer.Write(0x45564157); // "WAVE" in ASCII
        writer.Write(0x20746d66); // "fmt " in ASCII
        writer.Write(16);

        // write the format tag. 1 = PCM
        writer.Write((short)1);

        // write the number of channels.
        writer.Write((short)channels);

        // write the sample rate. 44100 in this case. The number of audio samples per second
        writer.Write(SAMPLE_RATE);

        writer.Write(SAMPLE_RATE * channels * (BITS_PER_SAMPLE / 8));
        writer.Write((short)(channels * (BITS_PER_SAMPLE / 8)));

        // 16 bits per sample
        writer.Write(BITS_PER_SAMPLE);

        // "data" in ASCII. Start the data chunk.
        writer.Write(0x61746164);

        // write the number of bytes in the data portion
        writer.Write((int)(numberOfSamples * BITS_PER_SAMPLE * channels / 8));

        // copy over the actual audio data
        this.outputStream.WriteTo(newOutputStream);

        // move the reference to the new stream
        this.outputStream = newOutputStream;
    }

    public void saveFile()
    {
        Debug.Log("start saving file");

        string path = Application.persistentDataPath + "/voice.wav";
        Save(path);

        Rendering = false;
    }

    private void Update()
    {
        if (GameState.Instance._mainCharacter == null) return;

        //if (Rendering)
        //    FillAudioBuffer();



    }

    void FillAudioBuffer()
    {
        //Debug.Log("FillAudioBuffer");
        //audioSource.GetOutputData(sampleDataArray, channels);
        //streamedClip.SetData(sampleDataArray, 0);
        //audioSource.clip.SetData(sampleDataArray, 0);
        //StartCoroutine(FillAudioBufferMainThread());
    }
    #endregion
}
