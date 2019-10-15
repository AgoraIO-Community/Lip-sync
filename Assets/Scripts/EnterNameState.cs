using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Diagnostics;
//using Photon.Voice.PUN;

public enum EServerRegion
{
    Auto,
    USWest,
    USEast,
    Singapore,
    China,
    EU,
    Japan

}

public class EnterNameState : BaseState
{

    //[SerializeField]
    //InputField m_userName;

    [SerializeField]
    Button m_joinRoomBtn;

    //[SerializeField]
    //private Dropdown _regionDropDown;

    //[SerializeField]
    //private Dropdown _microphoneDropDown;



    //private List<string> _regionList = new List<string>();

    // PRIVATE MEMBERS
    private bool micSelected = false;
    private int minFreq, maxFreq;
    private bool focused = true;
    private bool initialized = false;

    private int micFrequency = 48000;

    // Start is called before the first frame update
    void Start()
    {
        m_joinRoomBtn.onClick.AddListener(onJoinRoomButtonClicked);

        //currently remove region selector
        //foreach (EServerRegion region in System.Enum.GetValues(typeof(EServerRegion))) {

        //    if (region == EServerRegion.Auto)
        //        _regionCreator.initRegion(region, true);
        //    else
        //        _regionCreator.initRegion(region, false);
        //}

        //_regionDropDown.AddOptions(_regionList);

        //_regionDropDown.onValueChanged.AddListener(delegate {
        //    regionDropdownValueChanged();
        //});

        //List<string> stringList = new List<string>();
        //for (int i = 0; i < Microphone.devices.Length; i++) {
        //    stringList.Add(Microphone.devices[i]);
        //}
        //_microphoneDropDown.AddOptions(stringList);

        //_microphoneDropDown.onValueChanged.AddListener(delegate {
        //    microphoneDropdownValueChanged();
        //});
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnEnter()
    {
        base.OnEnter();

        //PopupManager.Instance.showSuccessWarningPopup(true);
    }

    public override void OnExit()
    {
        base.OnExit();


    }

    public void onJoinRoomButtonClicked() {

        uint udid = (uint)Random.Range(100000, 999999);

        GameState.Instance.UDID = udid;

        //GameState.Instance.connectPhotonRealtime();


        GameState.Instance.connectVoiceServer();

        //string username = m_userName.text.Trim();

        //if (username == string.Empty) return;

        //generate uid, optimize for real product


        PopupManager.Instance.showLoadingPopUp(true);
    }

    

    private void regionDropdownValueChanged() {
        //GameManager.Instance.currentRegion = (EServerRegion)_regionDropDown.value;
    }

    private void microphoneDropdownValueChanged() {
        //GameState.Instance.selectedDevice = Microphone.devices[_microphoneDropDown.value].ToString();
    }

    private void onTestButtonClicked() {

        GameState.Instance.selectedDevice = Microphone.devices[0].ToString();

        if (Microphone.IsRecording(GameState.Instance.selectedDevice))
        {
            Microphone.End(GameState.Instance.selectedDevice);
        }



        GetMicCaps();


        Microphone.Start(GameState.Instance.selectedDevice, true, 1, micFrequency);

        Stopwatch timer = Stopwatch.StartNew();

        // Wait until the recording has started
        while (!(Microphone.GetPosition(GameState.Instance.selectedDevice) > 0) && timer.Elapsed.TotalMilliseconds < 5000)
        {
            Thread.Sleep(50);
        }

        if (Microphone.GetPosition(GameState.Instance.selectedDevice) <= 0)
        {
            PopupManager.Instance.showWarningPopup(true);
            //throw new Exception("Timeout initializing microphone " + selectedDevice);
        }
        else
        {
            PopupManager.Instance.showSuccessWarningPopup(true);
        }
    }

    public void GetMicCaps()
    {
        if (micSelected == false) return;

        //Gets the frequency of the device
        Microphone.GetDeviceCaps(GameState.Instance.selectedDevice, out minFreq, out maxFreq);

        if (minFreq == 0 && maxFreq == 0)
        {
            UnityEngine.Debug.LogWarning("GetMicCaps warning:: min and max frequencies are 0");
            minFreq = 44100;
            maxFreq = 44100;
        }

        if (micFrequency > maxFreq)
            micFrequency = maxFreq;
    }
}
