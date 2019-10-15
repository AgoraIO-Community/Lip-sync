using System;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Threading;
using CrazyMinnow.SALSA;


[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour
{
    public enum micActivation
    {
        HoldToSpeak,
        PushToSpeak,
        ConstantSpeak
    }

    // PUBLIC MEMBERS
    [Tooltip("Manual specification of Audio Source - " +
        "by default will use any attached to the same object.")]
    public AudioSource audioSource = null;


    [Tooltip("Enable a keypress to toggle the microphone device selection GUI.")]
    public bool enableMicSelectionGUI = true;
    [Tooltip("Key to toggle the microphone selection GUI if enabled.")]
    public KeyCode micSelectionGUIKey = KeyCode.M;

    [SerializeField]
    [Range(0.0f, 100.0f)]
    [Tooltip("Microphone input volume control.")]
    private float micInputVolume = 100;

    [SerializeField]
    [Tooltip("Requested microphone input frequency")]
    private int micFrequency = 48000;
    public float MicFrequency
    {
        get { return micFrequency; }
        set { micFrequency = (int)Mathf.Clamp((float)value, 0, 96000); }
    }

    [Tooltip("Microphone input control method. Hold To Speak and Push" +
        " To Speak are driven with the Mic Activation Key.")]
    public micActivation micControl = micActivation.ConstantSpeak;
    [Tooltip("Key used to drive Hold To Speak and Push To Speak methods" +
        " of microphone input control.")]
    public KeyCode micActivationKey = KeyCode.Space;

    [Tooltip("Will contain the string name of the selected microphone device - read only.")]
    public string selectedDevice;

    public Salsa3D salsa3D;

    // PRIVATE MEMBERS
    private bool micSelected = false;
    private int minFreq, maxFreq;
    private bool focused = true;
    private bool initialized = false;

    private float restartCheckingMicTimer = 5;

    private bool enableCheckingMicDebuger = false;

    //----------------------------------------------------
    // MONOBEHAVIOUR OVERRIDE FUNCTIONS
    //----------------------------------------------------

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        // First thing to do, cache the unity audio source (can be managed by the
        // user if audio source can change)
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!audioSource) return; // this should never happen
    }

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start()
    {
        audioSource.loop = true;     // Set the AudioClip to loop
        audioSource.mute = false;

        InitializeMicrophone();

        StartMicrophone();
    }

    /// <summary>
    /// Initializes the microphone.
    /// </summary>
    private void InitializeMicrophone()
    {
        if (initialized)
        {
            return;
        }
        if (Microphone.devices.Length == 0)
        {
            return;
        }
        selectedDevice = Microphone.devices[0].ToString();
        micSelected = true;
        GetMicCaps();
        initialized = true;
    }

    private void InitializeMicrophone(string selecDevice)
    {
        if (initialized)
        {
            return;
        }
        if (Microphone.devices.Length == 0)
        {
            return;
        }
        selectedDevice = selecDevice;
        micSelected = true;
        GetMicCaps();
        initialized = true;
    }

    public void restartMicrophoneWithSelectedDevice(string newDevice) {
        InitializeMicrophone(newDevice);
        StartMicrophone();
    }

    //void OnGUI() {
    //Debug.Log("called " + Microphone.devices.Length);
    //for (int i = 0; i < Microphone.devices.Length;i++)
    //    Debug.Log(Microphone.devices[i].ToString());
    //MicDeviceGUI(50,50, 100, 50, 50, 50);
    //}

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {
        if (!focused)
        {
            if (Microphone.IsRecording(selectedDevice))
            {
                StopMicrophone();
            }
            return;
        }

        if (!Application.isPlaying)
        {
            StopMicrophone();
            return;
        }

        // Lazy Microphone initialization (needed on Android)
        if (!initialized)
        {
            InitializeMicrophone();
        }

        audioSource.volume = (micInputVolume / 100);

        //Hold To Speak
        if (micControl == micActivation.HoldToSpeak)
        {
            if (Input.GetKey(micActivationKey))
            {
                if (!Microphone.IsRecording(selectedDevice))
                {
                    StartMicrophone();
                }
            }
            else
            {
                if (Microphone.IsRecording(selectedDevice))
                {
                    StopMicrophone();
                }
            }
        }

        //Push To Talk
        if (micControl == micActivation.PushToSpeak)
        {
            if (Input.GetKeyDown(micActivationKey))
            {
                if (Microphone.IsRecording(selectedDevice))
                {
                    StopMicrophone();
                }
                else if (!Microphone.IsRecording(selectedDevice))
                {
                    StartMicrophone();
                }
            }
        }

        //Constant Speak
        if (micControl == micActivation.ConstantSpeak)
        {
            if (!Microphone.IsRecording(selectedDevice))
            {
                StartMicrophone();
            }
        }


        //Mic Selected = False
        if (enableMicSelectionGUI)
        {
            if (Input.GetKeyDown(micSelectionGUIKey))
            {
                micSelected = false;
            }
        }

        //if (enableCheckingMicDebuger) {
        //    restartCheckingMicTimer -= Time.deltaTime;

        //    if (restartCheckingMicTimer <= 0) {
        //        restartCheckingMicTimer = 5;
        //        StopMicrophone();
        //        StartMicrophone();
        //    }
        //}
    }


    /// <summary>
    /// Raises the application focus event.
    /// </summary>
    /// <param name="focus">If set to <c>true</c>: focused.</param>
    //void OnApplicationFocus(bool focus)
    //{
    //    focused = focus;

    //    if (!focused)
    //        StopMicrophone();
    //}

    /// <summary>
    /// Raises the application pause event.
    /// </summary>
    /// <param name="pauseStatus">If set to <c>true</c>: paused.</param>
    void OnApplicationPause(bool pauseStatus)
    {
        focused = !pauseStatus;

        if (!focused)
            StopMicrophone();
    }

    void OnDisable()
    {
        StopMicrophone();
    }

    /// <summary>
    /// Raises the GU event.
    /// </summary>


    private void restartCheckingMic()
    {
        for (int i = 0; i < Microphone.devices.Length; ++i)
        {
            if (GUI.Button(new Rect(80 * (i + 1),
                                    50, 150, 50),
                           Microphone.devices[i].ToString()))
            {
                StopMicrophone();
                selectedDevice = Microphone.devices[i].ToString();
                micSelected = true;
                GetMicCaps();
                StartMicrophone();
            }
        }
    }

    //----------------------------------------------------
    // PUBLIC FUNCTIONS
    //----------------------------------------------------

    /// <summary>
    /// Mics the device GU.
    /// </summary>
    /// <param name="left">Left.</param>
    /// <param name="top">Top.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <param name="buttonSpaceTop">Button space top.</param>
    /// <param name="buttonSpaceLeft">Button space left.</param>
    public void MicDeviceGUI(
        float left,
        float top,
        float width,
        float height,
        float buttonSpaceTop,
        float buttonSpaceLeft)
    {
        //If there is more than one device, choose one.
        if (Microphone.devices.Length >= 1 && enableMicSelectionGUI == true && micSelected == false)
        {
            for (int i = 0; i < Microphone.devices.Length; ++i)
            {
                if (GUI.Button(new Rect(80 * (i + 1),
                                        50, 150, 50),
                               Microphone.devices[i].ToString()))
                {
                    StopMicrophone();
                    selectedDevice = Microphone.devices[i].ToString();
                    micSelected = true;
                    GetMicCaps();
                    StartMicrophone();
                }
            }
        }
    }

    /// <summary>
    /// Gets the mic caps.
    /// </summary>
    public void GetMicCaps()
    {
        if (micSelected == false) return;

        //Gets the frequency of the device
        Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq);

        if (minFreq == 0 && maxFreq == 0)
        {
            Debug.LogWarning("GetMicCaps warning:: min and max frequencies are 0");
            minFreq = 44100;
            maxFreq = 44100;
        }

        if (micFrequency > maxFreq)
            micFrequency = maxFreq;
    }

    /// <summary>
    /// Starts the microphone.
    /// </summary>
    public void StartMicrophone()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

#if PHOTON_SOLUTION

        //do not use this mic if using photon voice
        if (GameState.Instance.streamingSolution == EStreamingSolution.PhotonVoice)
        {


            audioSource.clip = GameState.Instance.recorder.pMicWrapper.mic;

            audioSource.Play();

            return;
        }
#endif

        if (micSelected == false) return;

        //Starts recording
        audioSource.clip = Microphone.Start(selectedDevice, true, 1, micFrequency);

        Stopwatch timer = Stopwatch.StartNew();

        // Wait until the recording has started
        while (!(Microphone.GetPosition(selectedDevice) > 0) && timer.Elapsed.TotalMilliseconds < 1000)
        {
            Thread.Sleep(50);
        }

        //enableCheckingMicDebuger = true;

        if (Microphone.GetPosition(selectedDevice) <= 0)
        {
            //PopupManager.Instance.showWarningPopup(true);
            throw new Exception("Timeout initializing microphone " + selectedDevice);
        }
        else
        {
            //PopupManager.Instance.showSuccessWarningPopup(true);
        }

        // Play the audio source
         audioSource.Play();

    }




    /// <summary>
    /// Stops the microphone.
    /// </summary>
    public void StopMicrophone()
    {
#if PHOTON_SOLUTION
        //do not use this mic if using photon voice
        if (GameState.Instance.streamingSolution == EStreamingSolution.PhotonVoice) return;
#endif

        if (micSelected == false) return;

        // Overriden with a clip to play? Don't stop the audio source
        if ((audioSource != null) &&
            (audioSource.clip != null) &&
            (audioSource.clip.name == "Microphone"))
        {
            audioSource.Stop();
        }


        Microphone.End(selectedDevice);
    }


    //----------------------------------------------------
    // PRIVATE FUNCTIONS
    //----------------------------------------------------

    /// <summary>
    /// Gets the averaged volume.
    /// </summary>
    /// <returns>The averaged volume.</returns>
    float GetAveragedVolume()
    {
        // We will use the SR to get average volume
        // return OVRSpeechRec.GetAverageVolume();
        return 0.0f;
    }

    void OnApplicationQuit()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        Destroy(audioSource);

        Microphone.End(selectedDevice);
    }

}
