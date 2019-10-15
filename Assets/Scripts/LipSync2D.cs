//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Android;

//using CrazyMinnow.SALSA;

//using UnityEngine.UI;

//public class LipSync2D : MonoBehaviour
//{
//    [SerializeField]
//    private AudioSource _source;

//    [SerializeField]
//    private Salsa2D salsa2D; // Reference to the Salsa3D class

//    [SerializeField]
//    private Text nameTxt;

//    [SerializeField]
//    private SpriteRenderer mouthSpriteRenderer;

//    public uint ID;

//    bool isInit = false;

//    private float _delayTimer = 0.1f;

//    private float RmsValue;
//    private float DbValue;
//    private float PitchValue;

//    private const int QSamples = 1024;
//    private const float RefValue = 0.1f;
//    private const float Threshold = 0.02f;

//    float[] _samples;
//    private float[] _spectrum;
//    private float _fSample;

//    int sample = 128;
//    float Loudness = 0;

//    AudioClip _clip;

//    int _sampleWindow = 128;

//    float MicLoudness;

//    bool isReady = false;

//    //public void setUID(uint id) { ID = id; }

//    public List<Sprite> spriteList = new List<Sprite>();

//    // Start is called before the first frame update
//    void Start()
//    {
//        GameManager.Instance.onAudioMessageReceivedEvent += messageReceived;

//        //_samples = new float[QSamples];
//        //_spectrum = new float[QSamples];
//        //_fSample = 1024;
//    }

//    float LevelMax()
//    {
//        if (_clip == null) return 0;

//        float levelMax = 0;
//        float[] waveData = new float[_sampleWindow];
//        int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1); // null means the first microphone
//        if (micPosition < 0) return 0;
//        _clip.GetData(waveData, micPosition);
//        // Getting a peak on the last 128 samples
//        for (int i = 0; i < _sampleWindow; i++)
//        {
//            float wavePeak = waveData[i] * waveData[i];
//            if (levelMax < wavePeak)
//            {
//                levelMax = wavePeak;
//            }
//        }
//        return levelMax;
//    }


//    // Update is called once per frame
//    void Update()
//    {
//        if ( GameState.Instance.IsPermissionChecked == false && Permission.HasUserAuthorizedPermission(Permission.Microphone) && GameState.Instance.CurrentState == EGameState.ChatRoomState)
//        {
//            GameState.Instance.IsPermissionChecked = true;
//        }

//        if (!isReady && !isInit && GameState.Instance.IsPermissionChecked && !_source.isPlaying && GameState.Instance.CurrentState == EGameState.ChatRoomState)
//        {
//            isReady = true;
//            StartCoroutine(init());
//        }

//        //if (GameState.Instance.IsPermissionChecked && GameState.Instance.mRtcEngine != null && GameState.Instance.CurrentState == EGameState.ChatRoomState)
//        //{
//        //    GameState.Instance.mRtcEngine.Poll();
//        //}

//        //if (isInit && _delayTimer > 0)
//        //{
//        //    _delayTimer -= Time.deltaTime;

//        //    if (_delayTimer <= 0)
//        //    {

//        //        //AnalyzeSound();

//        //        Debug.Log("PitchValue is " + _source.pitch);

//        //        _delayTimer = 0.1f;
//        //    }
//        //}

//        //if (isInit) {
//        //    //AnalyzeSound();

//        //    if (_source.pitch > 1.1f)
//        //        Debug.Log("PitchValue is " + _source.pitch);
//        //}



//        //if (isInit)
//        //{
//        //    _delayTimer -= Time.deltaTime;

//        //    if (_delayTimer <= 0)
//        //    {

//        //        MicLoudness = LevelMax();
//        //        if (MicLoudness > 0.001f)
//        //        {
//        //            Debug.Log("MicLoudness " + Helper.ReformatFloat(MicLoudness, 2));

//        //            GameState.Instance.mRtcEngine.SendStreamMessage(123, "some data");

//        //            _delayTimer = 0.1f;
//        //        }


//        //    }
//        //}

//        //MicLoudness = LevelMax();
//        //if (MicLoudness > 0.001f)
//        //{
//        //    Debug.Log("MicLoudness " + Helper.ReformatFloat(MicLoudness, 2));

//        //    GameState.Instance.mRtcEngine.SendStreamMessage(123, "some data");

//        //    Debug.Log("SendStreamMessage " + Helper.ReformatFloat(MicLoudness, 2));

//        //    _delayTimer = 0.1f;
//        //}


//        //send stream message every 10ms
//        if (isInit)
//        {
//            _delayTimer -= Time.deltaTime;

//            if (_delayTimer <= 0)
//            {

//                //Debug.Log(salsa2D.sayIndex.ToString());


//                GameState.Instance.sendChannelMsg(salsa2D.sayIndex.ToString());

//                _delayTimer = 0.1f;
//            }
//        }
//    }


//    void AnalyzeSound()
//    {
//        _source.GetOutputData(_samples, 0); // fill array with samples
//        int i;
//        float sum = 0;
//        for (i = 0; i < QSamples; i++)
//        {
//            sum += _samples[i] * _samples[i]; // sum squared samples
//        }
//        RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average
//        DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
//        if (DbValue < -160) DbValue = -160; // clamp it to -160dB min
//                                            // get sound spectrum
//        _source.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
//        float maxV = 0;
//        var maxN = 0;
//        for (i = 0; i < QSamples; i++)
//        { // find max 
//            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
//                continue;

//            maxV = _spectrum[i];
//            maxN = i; // maxN is the index of max
//        }

//        if (maxV != 0) {
//            Debug.Log("maxV is " + maxV);
//        }
        

//        float freqN = maxN; // pass the index to a float variable
//        if (maxN > 0 && maxN < QSamples - 1)
//        { // interpolate index using neighbours
//            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
//            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
//            freqN += 0.5f * (dR * dR - dL * dL);
//        }
//        PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency

//        if (PitchValue != 0) {
//            Debug.Log("PitchValue is " + PitchValue);
//            Debug.Log("freqN is " + PitchValue);
//        }
  
//    }


//public void InitLipSync(uint id, string name, bool isSelf = false) {
//        ID = id;
//        //nameTxt.text = name;
//        nameTxt.text = id.ToString();

//    }

    

//    private void sendAudioMessage(AudioClip newClip) {

//        //debug
//        return;

//        //send this audio clip to other users
//        float[] samples = new float[newClip.samples * newClip.channels];

//        Debug.Log(newClip.samples + "," + newClip.channels);

//        newClip.GetData(samples, 0);

//        CustomAudioData newAudioData = new CustomAudioData();

//        //data structure
//        //streamID = random id
//        int streamID = (int)Random.Range(100000, 999999);

//        newAudioData.StreamID = streamID;
//        newAudioData.Samples = Helper.ToByteArray(samples);
//        newAudioData.OffsetSample = 0;
//        newAudioData.LengthSamples = samples.Length;
//        newAudioData.Channels = newClip.channels;
//        newAudioData.Frequency = newClip.frequency;

//        string audioDataString = JsonUtility.ToJson(newAudioData);

//        GameState.Instance.mRtcEngine.SendStreamMessage(streamID, audioDataString);
//    }

//    private IEnumerator init()
//    {
//        if (isInit) yield break;

//        if (spriteList == null)
//            spriteList = new List<Sprite>();

//        spriteList.Clear();

//        spriteList.Add(salsa2D.sayRestSprite);
//        spriteList.Add(salsa2D.saySmallSprite);
//        spriteList.Add(salsa2D.sayMediumSprite);
//        spriteList.Add(salsa2D.sayLargeSprite);

//        if (ID != GameState.Instance.UDID) yield break;



//        _clip = Microphone.Start(null, true, 10, 44100);
//        _source.clip = _clip;
//        _source.loop = true;
//        salsa2D.SetAudioClip(_clip);


//        while (!(Microphone.GetPosition(null) > 0) )
//        {
//            yield return null;
//        }

//        //sendAudioMessage(newClip);

//        //_source.Play();
//        //_source.mute = true; // Mute audio ...

//        //salsa2D.Stop();


//        salsa2D.Play();


        

//        isInit = true;
//    }

//    private void messageReceived(uint uid, int index) {

//        if (uid != ID) return;

//        mouthSpriteRenderer.sprite = spriteList[index];
//    }

//    void OnDestroy()
//    {
//        Debug.Log("OnDestroy");
//        Microphone.End(null);
//        _clip = null;
//        GameManager.Instance.onAudioMessageReceivedEvent -= messageReceived;

//        if (GameState.Instance.mRtcEngine != null)
//            GameState.Instance.mRtcEngine.LeaveChannel();
//    }
//}
