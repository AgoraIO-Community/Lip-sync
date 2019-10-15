using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if PHOTON_SOLUTION
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
#endif

using agora_gaming_rtc;
using UnityEngine.UI;

#if (UNITY_2018_3_OR_NEWER)
using UnityEngine.Android;
#endif

//using Photon.Voice.Unity;
//using Photon.Voice.PUN;

using UnityEngine.SceneManagement;

//using CrazyMinnow.SALSA;

public enum EGameState
{
    EnterNameState,
    ChatRoomState
}

public enum EStreamingSolution {
    Agora,
#if PHOTON_SOLUTION
    PhotonVoice
#endif
}
public class GameState : MonoSingleton<GameState>
{

    public event Action<EGameState> OnStateChange;

    [SerializeField]
    private EGameState m_DefaultState;

    [SerializeField]
    private BaseState m_enterNameState;

    [SerializeField]
    private BaseState m_chatRoomState;

    [SerializeField]
    private string appId = "4133ccb0f5c5455a98d8d2a9ef00dc4b";

    [SerializeField]
    private Text punState;

    [SerializeField]
    private Text voiceState;

    [SerializeField]
    private Text voiceDebugText;

    [SerializeField]
    private Text devicesInfoText;

    //[SerializeField]
    //private Dropdown _micSelectorDropDown;

    public NetworkCharacter _mainCharacter;

    public EGameState CurrentState { get; private set; }

    private readonly Stack<EGameState> m_StateStack = new Stack<EGameState>();
    private BaseState m_CurrentState;

    public IRtcEngine mRtcEngine = null;

    //public agora_gaming_rtc engine = null;


    string debugTag = "gamestate ";

    // Use this for initialization
    private ArrayList permissionList = new ArrayList();

    public bool IsPermissionChecked = false;

    private float[] buffData;

    //private Salsa2D salsa2D; // Reference to the Salsa3D class

    public uint UDID;

    public int mDataStreamId;

    public List<NetworkCharacter> _networkCharacterList = new List<NetworkCharacter>();

    public string selectedDevice;

    public Transform startPoint;

    public Transform startPoint2;

    public Transform startPoint3;

    public EStreamingSolution streamingSolution = EStreamingSolution.Agora; //agora as default

    public ELipSyncSolution lipSyncSolution = ELipSyncSolution.SalsaSolution;

    //public Recorder recorder = null;

    //private PhotonVoiceView photonVoiceView = null;

    private MicrophoneInput micInput;

    AndroidJavaClass activityClass;
    AndroidJavaObject chatActivityObject;

    public OVRLipSyncContext ovr_context;

    private AudioRenderer audiorenderer;

    private AudioRawDataManager audioRawDataManager = null;

    private bool isLittleEndian = false;

    private AudioClip clipTemp = null;

    private int position = 0;

    private void Awake()
    {



    }

    //private void OnAudioRead(float[] data) {

        

    //    //salsa works on 2 people only
    //    data = _networkCharacterList[0].audioDataBuffer;

    //    Debug.Log("OnAudioRead " + _networkCharacterList[0].audioDataBuffer.Length);

    //    //for (int i = 0; i < data.Length; i++) {
    //    //    data[i] *= 1000;
    //    //    if (data[i] > 0.2f)
    //    //        Debug.Log(data[i]);
    //    //}

    //    position += data.Length;

    //    //_networkCharacterList[0].salsaComponent.audioClip.SetData(data, 0);
    //    //_networkCharacterList[0].salsaComponent.Play();
    //}

    private void PCMSetPositionCallback(int pos)
    {
        position = pos;
    }


    private void OnEnable() {
#if PHOTON_SOLUTION
        PhotonNetwork.NetworkingClient.StateChanged += PunClientStateChanged;

        if (PhotonVoiceNetwork.Instance)
            PhotonVoiceNetwork.Instance.Client.StateChanged += VoiceClientStateChanged;
#endif
    }

    private void OnDisable() {
#if PHOTON_SOLUTION
        PhotonNetwork.NetworkingClient.StateChanged -= PunClientStateChanged;

        if (PhotonVoiceNetwork.Instance)
            PhotonVoiceNetwork.Instance.Client.StateChanged -= VoiceClientStateChanged;
#endif
    }

    private void Start()
    {
        m_StateStack.Push(m_DefaultState);
        SetState(m_DefaultState);

        showAllDevice();

#if (UNITY_2018_3_OR_NEWER)
        permissionList.Add(Permission.Microphone);
#endif

        CheckPermission();

        initEngine();




    }

    private void bufferDataReceived(string data) {
        //Debug.Log("bufferDataReceived " + data);

        byte[] byteArray = Convert.FromBase64String(data);

        buffData = Helper.PCM2Floats(byteArray);

        if (ovr_context != null) {
            ovr_context.ProcessAudioSamples(buffData, 2);
        }
            
    }

    private void showAllDevice() {
        if (devicesInfoText != null)
        {
            if (Microphone.devices == null || Microphone.devices.Length == 0)
            {
                devicesInfoText.enabled = true;
                devicesInfoText.color = Color.red;
                devicesInfoText.text = "No microphone device detected!";
            }
            else if (Microphone.devices.Length == 1)
            {
                devicesInfoText.text = string.Format("Mic.: {0}", Microphone.devices[0]);
            }
            else
            {
                devicesInfoText.text = string.Format("Multi.Mic.Devices:\n0. {0} (active)\n", Microphone.devices[0]);
                for (int i = 1; i < Microphone.devices.Length; i++)
                {
                    devicesInfoText.text = string.Concat(devicesInfoText.text, string.Format("{0}. {1}\n", i, Microphone.devices[i]));
                }
            }
        }
    }

    public void connectPhotonRealtime() {
#if PHOTON_SOLUTION
        switch (GameManager.Instance.currentRegion)
        {
            case EServerRegion.Auto:
                PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
                break;
            case EServerRegion.Singapore:
                PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
                break;
            case EServerRegion.USEast:
                PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us";
                break;
            case EServerRegion.USWest:
                PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "usw";
                break;
            case EServerRegion.EU:
                PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
                break;
            case EServerRegion.China:
                PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "cn";
                break;
            case EServerRegion.Japan:
                PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "jp";
                break;
        }

        PhotonNetwork.ConnectUsingSettings();
#endif

    }

    public void reloadEngine() {

        unloadEngine();

        initEngine();

    }

    public void unloadEngine() {
        if (streamingSolution == EStreamingSolution.Agora && mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            mRtcEngine = null;
        }
    }

    public void initEngine() {
        if (streamingSolution == EStreamingSolution.Agora)
            initAgoraEngine();
        else
            initPhotonVoice();

    }

    public void connectPhotonVoiceServer() {
        //if (PhotonVoiceNetwork.Instance.ClientState == Photon.Realtime.ClientState.Joined)
        //{
        //    PhotonVoiceNetwork.Instance.Disconnect();
        //}
        //else if (PhotonVoiceNetwork.Instance.ClientState == Photon.Realtime.ClientState.PeerCreated
        //         || PhotonVoiceNetwork.Instance.ClientState == Photon.Realtime.ClientState.Disconnected)
        //{
        //    PhotonVoiceNetwork.Instance.ConnectAndJoinRoom();
        //}
    }

    public void connectVoiceServer() {

        if (streamingSolution == EStreamingSolution.Agora)
        {
            joinAgoraChannel();
        }

#if PHOTON_SOLUTION

        else if (streamingSolution == EStreamingSolution.PhotonVoice)
        {
            connectPhotonVoiceServer();
        }
#endif
    }

    public void joinAgoraChannel()
    {
        if (mRtcEngine == null)
        {
            UnityEngine.Debug.LogError("mRtcEngine is null");
            return;
        }

        audioRawDataManager.RegisteAudioRawDataObserver();

        Debug.Log("join channel with uid " + GameState.Instance.UDID);

        //everyone join test channel
        mRtcEngine.JoinChannel("silicon", string.Empty, GameState.Instance.UDID);
    }

    public void initPhotonVoice() {
        Debug.Log("init photon voice");
    }

    public void initAgoraEngine() {

        Debug.Log("init agora engine");

        mRtcEngine = IRtcEngine.GetEngine(appId);

        audioRawDataManager = AudioRawDataManager.GetInstance(mRtcEngine);
        audioRawDataManager.SetOnMixedAudioFrameCallback(OnMixedAudioFrameHandler);
        audioRawDataManager.SetOnPlaybackAudioFrameBeforeMixingCallback(OnPlaybackAudioFrameBeforeMixingHandler);
        audioRawDataManager.SetOnPlaybackAudioFrameCallback(OnPlaybackAudioFrameHandler);
        audioRawDataManager.SetOnRecordAudioFrameCallback(OnRecordAudioFrameHandler);

        // mRtcEngine.SetLogFilter(LOG_FILTER.INFO);

        // mRtcEngine.setLogFile("path_to_file_unity.log");

        //mRtcEngine.EnableDualStreamMode(true);
        mRtcEngine.SetClientRole(CLIENT_ROLE.BROADCASTER);

        //mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.GAME_FREE_MODE);

        mDataStreamId = GameState.Instance.mRtcEngine.CreateDataStream(true, true);

        mRtcEngine.OnJoinChannelSuccess += (string channelName, uint uid, int elapsed) => {
            string joinSuccessMessage = string.Format(debugTag + "joinChannel callback uid: {0}, channel: {1}, version: {2}", uid, channelName, IRtcEngine.GetSdkVersion());
            //Debug.Log(joinSuccessMessage);

            Debug.Log("OnJoinChannelSuccess " + uid);

            UDID = uid;

            createCharacter(uid, true);

            GameState.Instance.PushState(EGameState.ChatRoomState);

            PopupManager.Instance.showLoadingPopUp(false);


        };

        //mRtcEngine.EnableVideo();
        //mRtcEngine.EnableVideoObserver();

        mRtcEngine.OnLeaveChannel += (RtcStats stats) => {
            string leaveChannelMessage = string.Format(debugTag + "onLeaveChannel callback duration {0}, tx: {1}, rx: {2}, tx kbps: {3}, rx kbps: {4}", stats.duration, stats.txBytes, stats.rxBytes, stats.txKBitRate, stats.rxKBitRate);
            //Debug.Log(leaveChannelMessage);



            //reloadEngine();
            audioRawDataManager.UnRegisteAudioRawDataObserver();
        };

        mRtcEngine.OnUserJoined += (uint uid, int elapsed) => {
            string userJoinedMessage = string.Format(debugTag + "onUserJoined callback uid {0} {1}", uid, elapsed);
            //Debug.Log(userJoinedMessage);

            Debug.Log("on user joined " + uid);

            //remote client
            //GameManager.Instance.createAvatar(uid, string.Empty);
            NetworkCharacter character = createCharacter(uid);

            if (lipSyncSolution == ELipSyncSolution.SalsaSolution) {
                int samplerate = 44100;

                clipTemp = AudioClip.Create("clipTemp", samplerate * 2, 1, samplerate, false);

                character.audioSource.clip = clipTemp;
                character.audioSource.loop = true;

                character.salsaComponent.audioSrc = character.audioSource;

                character.salsaComponent.SetAudioClip(character.audioSource.clip);

                

                character.audioSource.Play();



                //character.salsaComponent.Play();

                //character.audioSource.clip = clipTemp;


                //character.audioSource.Play();

                //character.salsaComponent.audioClip = clipTemp;
                character.salsaComponent.Play();
            }

            
        };

        mRtcEngine.OnUserOffline += (uint uid, USER_OFFLINE_REASON reason) => {
            string userOfflineMessage = string.Format(debugTag + "onUserOffline callback uid {0} {1}", uid, reason);
            //Debug.Log(userOfflineMessage);

            NetworkCharacter character = getCharacterFromID(uid);

            Destroy(character.gameObject);

            GameManager.Instance.UserLeaveChannel(uid);
        };

        mRtcEngine.OnVolumeIndication += (AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume) => {
            if (speakerNumber == 0 || speakers == null)
            {
                //Debug.Log(string.Format(debugTag + "onVolumeIndication only local {0}", totalVolume));
            }

            for (int idx = 0; idx < speakerNumber; idx++)
            {
                string volumeIndicationMessage = string.Format(debugTag + "{0} onVolumeIndication {1} {2}", speakerNumber, speakers[idx].uid, speakers[idx].volume);
                //Debug.Log(volumeIndicationMessage);
            }
        };

        mRtcEngine.OnStreamMessageError += (uint userId, int streamId, int code, int missed, int cached) =>
        {
            Debug.Log("OnStreamMessageError " + code);

        };

        mRtcEngine.OnStreamMessage += (uint uid, int streamId, string data, int length) => {
            NetworkCharacter networkCharacter = getCharacterFromID(uid);

            Debug.Log("receive OnStreamMessage");

            networkCharacter.ReceiveLipsyncMessage(data);
        };

        mRtcEngine.OnUserMuted += (uint uid, bool muted) => {
            string userMutedMessage = string.Format(debugTag + "onUserMuted callback uid {0} {1}", uid, muted);
            //Debug.Log(userMutedMessage);
        };

        mRtcEngine.OnWarning += (int warn, string msg) => {
            string description = IRtcEngine.GetErrorDescription(warn);
            string warningMessage = string.Format(debugTag + "onWarning callback {0} {1} {2}", warn, msg, description);
            //Debug.Log(warningMessage);
        };

        mRtcEngine.OnError += (int error, string msg) => {
            string description = IRtcEngine.GetErrorDescription(error);
            string errorMessage = string.Format(debugTag + "onError callback {0} {1} {2}", error, msg, description);
            //Debug.Log(errorMessage);
        };

        mRtcEngine.OnRtcStats += (RtcStats stats) => {
            string rtcStatsMessage = string.Format(debugTag + "onRtcStats callback duration {0}, tx: {1}, rx: {2}, tx kbps: {3}, rx kbps: {4}, tx(a) kbps: {5}, rx(a) kbps: {6} users {7}",
                stats.duration, stats.txBytes, stats.rxBytes, stats.txKBitRate, stats.rxKBitRate, stats.txAudioKBitRate, stats.rxAudioKBitRate, stats.users);
            //Debug.Log(rtcStatsMessage);

            int lengthOfMixingFile = mRtcEngine.GetAudioMixingDuration();
            int currentTs = mRtcEngine.GetAudioMixingCurrentPosition();

            //string mixingMessage = string.Format(debugTag + "Mixing File Meta {0}, {1}", lengthOfMixingFile, currentTs);
           // Debug.Log(mixingMessage);
        };

        mRtcEngine.OnAudioRouteChanged += (AUDIO_ROUTE route) => {
           //string routeMessage = string.Format(debugTag + "onAudioRouteChanged {0}", route);
            //Debug.Log(routeMessage);
        };

        mRtcEngine.OnRequestToken += () => {
            string requestKeyMessage = string.Format(debugTag + "OnRequestToken");
            //Debug.Log(requestKeyMessage);
        };

        mRtcEngine.OnConnectionInterrupted += () => {
            string interruptedMessage = string.Format(debugTag + "OnConnectionInterrupted");
            //Debug.Log(interruptedMessage);
        };

        mRtcEngine.OnConnectionLost += () => {
            string lostMessage = string.Format(debugTag + "OnConnectionLost");
            //Debug.Log(lostMessage);
        };




    }

    //private bool isUserAlreadyRegistered(uint uid) {
    //    for (int i = 0; i < _userInfoList.Count; i++) {
    //        if (_userInfoList[i].userID == uid)
    //            return true;
    //    }
    //    return false;
    //}

    public void onJoinChannelSuccess(string nativeUid) {

        //uint uid = UInt32.Parse(nativeUid);

        //Debug.Log("dallin onJoinChannelSuccess Unity " + uid);

        //UDID = uid;

        //createCharacter(uid, true);

        //GameState.Instance.PushState(EGameState.ChatRoomState);

        //PopupManager.Instance.showLoadingPopUp(false);
    }

    public void onUserJoined(string nativeUid) {

        //uint uid = UInt32.Parse(nativeUid);

        //createCharacter(uid);
    }

    public void onStreamMessageNative(string data) {
        //take uid and data

        string[] splitData = data.Split(new string[] { "*data*" }, StringSplitOptions.None);

        string uidString = splitData[0];

        //convert uid to uint uid
        uint uid = UInt32.Parse(uidString);

        string justData = splitData[1];

        NetworkCharacter character = getCharacterFromID(uid);

        character.ReceiveLipsyncMessage(justData);
    }

    public void sendChannelMsgNative(string message)
    {
        chatActivityObject.Call<AndroidJavaObject>("sendChannelMsg", message);

    }

    public void sendChannelMsg(string message) {

        Debug.Log("sendChannelMsg");

        if (mDataStreamId <= 0)
        {
            mDataStreamId = mRtcEngine.CreateDataStream(true, true); // boolean reliable, boolean ordered
        }

        if (mDataStreamId < 0)
        {
            String errorMsg = "onstreammessage Create data stream error happened " + mDataStreamId;
            Debug.LogError(errorMsg);
            return;
        }

        mRtcEngine.SendStreamMessage(mDataStreamId, message);

    }

    private NetworkCharacter createCharacter(uint udid, bool isMine = false) {

        GameObject characterPrefab = null;
        NetworkCharacter networkCharacter = null;

        try {
            if (lipSyncSolution == ELipSyncSolution.OculusSolution)
            {
                characterPrefab = Resources.Load<GameObject>("Character");
            }
            else if (lipSyncSolution == ELipSyncSolution.SalsaSolution)
            {
                characterPrefab = Resources.Load<GameObject>("Ethan");
            }
        }
        catch (Exception e) {
            Debug.LogError("can not load character from resources");
            return null;
        }

        if (isMine)
        {
            GameObject characterObj = Instantiate(characterPrefab, startPoint);

            characterObj.SetActive(true);

            characterObj.transform.localPosition = Vector3.zero;
            characterObj.transform.localRotation = Quaternion.identity;

            networkCharacter = characterObj.GetComponent<NetworkCharacter>();
            _mainCharacter = networkCharacter;

            networkCharacter.initCharacter(udid, true);
        }
        else
        {
            GameObject characterObj = null;

            if (lipSyncSolution == ELipSyncSolution.OculusSolution)
                characterObj = Instantiate(characterPrefab, startPoint3);
            else if (lipSyncSolution == ELipSyncSolution.SalsaSolution)
                characterObj = Instantiate(characterPrefab, startPoint2);

            characterObj.SetActive(true);

            if (characterObj == null)
                Debug.LogError("this lip sync solution is not implemented");

            characterObj.transform.localPosition = Vector3.zero;
            characterObj.transform.localRotation = Quaternion.Euler(0, 180, 0);

            networkCharacter = characterObj.GetComponent<NetworkCharacter>();

            _networkCharacterList.Add(networkCharacter);

            networkCharacter.initCharacter(udid);
        }

        return networkCharacter;
       
    }


    private void CheckPermission()
    {
#if (UNITY_2018_3_OR_NEWER)
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            // The user authorized use of the microphone.
        }
        else
        {
            StartCoroutine(AskPermission());

            // We do not have permission to use the microphone.
            // Ask for permission or proceed without the functionality enabled.



        }
        
#endif
    }

    IEnumerator AskPermission()
    {
        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            SceneManager.LoadSceneAsync(0);
            yield break;
        }



        Application.RequestUserAuthorization(UserAuthorization.Microphone);
        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            SceneManager.LoadSceneAsync(0);
            yield break;
        }
        else {
            yield return null;
        }
            
    }


    private void Update()
    {
        if (m_CurrentState)
            m_CurrentState.OnUpdate();

        if (Input.GetKeyDown("space"))
        {
            Debug.Log("space keyboard press");
            _mainCharacter.switchMyHead();
        }

        //if (!IsPermissionChecked && Permission.HasUserAuthorizedPermission(Permission.Microphone)) {
        //    IsPermissionChecked = true;

        //    //reload scene

        //}

        if (_mainCharacter == null) return;

        //if (recorder && recorder.LevelMeter != null)
        //{
        //    voiceDebugText.text = string.Format("Amp: avg. {0:0.000000}, peak {1:0.000000}", recorder.LevelMeter.CurrentAvgAmp, recorder.LevelMeter.CurrentPeakAmp);
        //}




    }

    public void initRecorder() {

        Debug.Log("initRecorder");



        //if (photonVoiceView == null)
        //    photonVoiceView = _mainCharacter.GetComponent<PhotonVoiceView>();

        //if (photonVoiceView != null)
        //    Debug.Log("initRecorder 2");

        //if (photonVoiceView.IsRecorder)
        //{
        //    Debug.Log("initRecorder 3");

        //    recorder = photonVoiceView.RecorderInUse;

        //    recorder.TransmitEnabled = true;
        //    recorder.VoiceDetection = true;
        //    recorder.DebugEchoMode = true;
        //}
    }

    

    private void OnGUI()
    {
        if (m_CurrentState)
            m_CurrentState.OnDraw();
    }

    public void PushState(EGameState state)
    {
        if (CurrentState == state)
            return;

        m_StateStack.Push(state);
        SetState(state);
    }

    public void PopState()
    {
        if (m_StateStack.Count <= 1)
        {
            Debug.LogError("No previous state");
            return;
        }

        m_StateStack.Pop();
        EGameState state = m_StateStack.Peek();
        SetState(state);
    }

    public void PopStateUntilReach(params EGameState[] reeachState)
    {
        EGameState state = m_StateStack.Peek();

        while (!IsContainState(state, reeachState) && m_StateStack.Count > 1)
        {
            m_StateStack.Pop();
            state = m_StateStack.Peek();
        }

        SetState(state);
    }

    public void PopStateUntilReachThenPush(EGameState state, params EGameState[] reachState)
    {
        EGameState checkState = m_StateStack.Peek();

        while (!IsContainState(checkState, reachState) && m_StateStack.Count > 1)
        {
            m_StateStack.Pop();
            checkState = m_StateStack.Peek();
        }

        PushState(state);
    }

    public void PopStateThenPush(EGameState state)
    {
        if (m_StateStack.Count <= 1)
        {
            Debug.LogError("No previous state");
            return;
        }

        m_StateStack.Pop();
        m_StateStack.Push(state);
        SetState(state);
    }

    private bool IsContainState(EGameState state, params EGameState[] stateArray)
    {
        for (int i = 0; i < stateArray.Length; i++)
        {
            if (stateArray[i] == state)
                return true;
        }

        return false;
    }

    private void SetState(EGameState state)
    {
        CurrentState = state;

        if (m_CurrentState)
            m_CurrentState.OnExit();

        switch (state)
        {
            case EGameState.EnterNameState:
                m_CurrentState = m_enterNameState;
                break;

            case EGameState.ChatRoomState:
                m_CurrentState = m_chatRoomState;
                break;

           
        }

        if (m_CurrentState)
            m_CurrentState.OnEnter();

        if (OnStateChange != null)
            OnStateChange(CurrentState);
    }

    public BaseState GetStateObject(EGameState state)
    {
        switch (state)
        {
            case EGameState.EnterNameState:
                return m_enterNameState;

            case EGameState.ChatRoomState:
                return m_chatRoomState;
        }

        return null;
    }
    void OnApplicationQuit()
    {
        audioRawDataManager.UnRegisteAudioRawDataObserver();
        mRtcEngine.LeaveChannel();
        unloadEngine();
        
    }

    //from -1 to 1
    public double getPanFromUser(uint userID)
    {
        double result = -1;

        NetworkCharacter character = getCharacterFromID(userID);

        if (character == null) return -1; // get this value, no need to set anything

        Vector3 targetDir = character.transform.position - _mainCharacter.transform.position;
        double angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

        result = angle / 180;

        return result;
    }

    //from 0 to 100
    public double getGainFromUser(uint userID)
    {
        double result = -1;

        float maxDistance = GameManager.Instance.maxDistanceToHear;

        NetworkCharacter character = getCharacterFromID(userID);

        if (character == null) return -1; // get this value, no need to set anything

        double distance = Vector3.Distance(character.transform.position, _mainCharacter.transform.position);

        result = 1 - distance / maxDistance;

        if (result < 0)
            result = 0;

        result *= 100;
        result = Math.Floor(result);

        return result;
    }

    private NetworkCharacter getCharacterFromID(uint id) {

        if (_networkCharacterList == null || _networkCharacterList.Count <= 0) return null;


        for (int i = 0; i < _networkCharacterList.Count; i++)
        {
            if (_networkCharacterList[i].UDID == id)
                return _networkCharacterList[i];
        }

        return null;
    }

    private void getMicrophoneInput() {
        if (lipSyncSolution == ELipSyncSolution.OculusSolution)
        {
            micInput = _mainCharacter.GetComponentInChildren<MicrophoneInput>();
        }
        else if (lipSyncSolution == ELipSyncSolution.SalsaSolution) {
            micInput = _mainCharacter.GetComponent<MicrophoneInput>();
        }
    }
#if PHOTON_SOLUTION
    private void PunClientStateChanged(Photon.Realtime.ClientState fromState, Photon.Realtime.ClientState toState)
    {
        punState.text = string.Format("PUN: {0}", toState);

        //if (PhotonVoiceNetwork.Instance)
        //    UpdateUiBasedOnVoiceState(PhotonVoiceNetwork.Instance.ClientState);
    }

    private void VoiceClientStateChanged(Photon.Realtime.ClientState fromState, Photon.Realtime.ClientState toState)
    {
        UpdateUiBasedOnVoiceState(toState);
    }

    private void UpdateUiBasedOnVoiceState(Photon.Realtime.ClientState voiceClientState)
    {
        if (voiceState)
            voiceState.text = string.Format("PhotonVoice: {0}", voiceClientState);

        
    }

#endif



    public void onRecordingButtonClicked() {
        if (audiorenderer)
            audiorenderer.StartRecording();
    }

    public void onSaveRecordButtonClicked() {
        if (audiorenderer)
            audiorenderer.saveFile();
    }


    public void OnRecordAudioFrameHandler(AudioFrame audioFrame)
    {
        //Debug.Log("AgoraTest  OnRecordAudioFrameHandler  buffer = " + audioFrame.buffer + " ,bytesPerSample = " + audioFrame.bytesPerSample + ",channels = " + audioFrame.channels + ",renderTimeMs = " + audioFrame.renderTimeMs + ",samples = " + audioFrame.samples + ",samplesPerSec = " + audioFrame.samplesPerSec + ",type = " + audioFrame.type + ",avsync_type = " + audioFrame.avsync_type);
    }

    public void OnPlaybackAudioFrameHandler(AudioFrame audioFrame)
    {
        //Debug.Log("AgoraTest  OnPlaybackAudioFrameHandler  buffer = " + audioFrame.buffer + " ,bytesPerSample = " + audioFrame.bytesPerSample + ",channels = " + audioFrame.channels + ",renderTimeMs = " + audioFrame.renderTimeMs + ",samples = " + audioFrame.samples + ",samplesPerSec = " + audioFrame.samplesPerSec + ",type = " + audioFrame.type + ",avsync_type = " + audioFrame.avsync_type);
    }

    public void OnMixedAudioFrameHandler(AudioFrame audioFrame)
    {
        //Debug.Log("AgoraTest  OnMixedAudioFrameHandler  buffer = " + audioFrame.buffer + " ,bytesPerSample = " + audioFrame.bytesPerSample + ",channels = " + audioFrame.channels + ",renderTimeMs = " + audioFrame.renderTimeMs + ",samples = " + audioFrame.samples + ",samplesPerSec = " + audioFrame.samplesPerSec + ",type = " + audioFrame.type + ",avsync_type = " + audioFrame.avsync_type);
    }


    public void OnPlaybackAudioFrameBeforeMixingHandler(uint uid, AudioFrame audioFrame)
    {

        //Debug.Log("AgoraTest  OnPlaybackAudioFrameBeforeMixingHandler  buffer = " + audioFrame.buffer + " ,bytesPerSample = " + audioFrame.bytesPerSample + ",channels = " + audioFrame.channels + ",renderTimeMs = " + audioFrame.renderTimeMs + ",samples = " + audioFrame.samples + ",samplesPerSec = " + audioFrame.samplesPerSec + ",type = " + audioFrame.type + ",avsync_type = " + audioFrame.avsync_type);

        //if (uid != UDID) return;




        NetworkCharacter character = getCharacterFromID(uid);

        if (character == null) return;

        

        byte[] byteArray = audioFrame.buffer;

        buffData = Helper.PCM2Floats(byteArray, isLittleEndian);

        if (lipSyncSolution == ELipSyncSolution.OculusSolution && character.context != null)
        {
            character.context.ProcessAudioSamples(buffData, 2);
        }
        else if (lipSyncSolution == ELipSyncSolution.SalsaSolution && character.salsaComponent != null) {

            //Debug.Log(buffData.Length);

            //character.audioSource.clip = clipTemp;
            //character.audioSource.Play();

            //Debug.Log("set data " + character.UDID + " " + buffData.Length);

            character.audioDataBuffer = buffData;

            //if (character.audioSource && character.audioSource.clip) {

            //    if (character.audioSource.isPlaying)
            //        character.audioSource.Stop();

            //    character.audioSource.clip.SetData(buffData, 0);
            //    character.audioSource.Play();
            //}

            //Debug.Log("set buffer data");


        }

    }

    //public void JoinChannel()
    //{
    //    //string channelName = mChannelNameInputField.text.Trim();

    //    Debug.Log(string.Format("tap joinChannel with channel name {0}", channelName));

    //    if (string.IsNullOrEmpty(channelName))
    //    {
    //        return;
    //    }

    //    mRtcEngine.JoinChannel(channelName, "extra", 0);
    //    // mRtcEngine.JoinChannelByKey ("YOUR_CHANNEL_KEY", channelName, "extra", 9527);
    //}

    public void littleEndianButtonClick() {
        isLittleEndian = true;
    }

    public void noneLittleEndianButtonClicked() {
        isLittleEndian = false;
    }

}