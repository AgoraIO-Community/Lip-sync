using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if PHOTON_SOLUTION
using Photon.Pun;
using Photon.Realtime;
#endif


using UnityStandardAssets.Cameras;
using agora_gaming_rtc;
using CrazyMinnow.SALSA;

#if PHOTON_SOLUTION
public class NetworkCharacter : MonoBehaviourPun, IPunInstantiateMagicCallback, IPunObservable
#endif

public class NetworkCharacter : MonoBehaviour
{
    //references
    public GameObject characterCamera;
    public OVRLipSyncContext context;
    public MicrophoneInput micInput;
    public AudioSource audioSource;
    public OVRLipSyncContextMorphTarget morphTarget;
    public OVRLipSyncContextTextureFlip textureFlip;

    public SkinnedMeshRenderer skinMeshRenderer;

    public Salsa3D salsaComponent;

    public RandomEyes3D randomEyesComponent;

    public float[] audioDataBuffer;

    //transferred data
    public string frameData;
    public int smoothAmount;

    public uint UDID;

    private EHeadType currentType = EHeadType.Human;

    private SwitchHead switchHead = null;

    private Mesh skinMesh;

    SalsaBlendShape salsaBlendShape;

    bool m_IsMine = false;

    private float timerToPlayClip = 0.1f; 


    private void Start()
    {
        switchHead = GetComponent<SwitchHead>();

        skinMesh = skinMeshRenderer.sharedMesh;

        salsaBlendShape = new SalsaBlendShape();

#if PHOTON_SOLUTION
        photonView.ObservedComponents.Add(this);
#endif
        
    }

    public void restartMicrophone(string newDevice) {
        micInput.restartMicrophoneWithSelectedDevice(newDevice);
    }

    public void initCharacter(uint udid, bool isMine = false)
    {
        m_IsMine = isMine;
        UDID = udid;

        if (isMine)
        {
            //Transform[] positions = GameManager.Instance.photonManager.transform.GetComponentsInChildren<Transform>();
            //transform.position = positions[Random.Range(0, positions.Length)].position;

            Transform[] childs = GetComponentsInChildren<Transform>();
            foreach (var child in childs)
            {
                child.gameObject.layer = LayerMask.NameToLayer("SelfOcclude");
            }

            GameState.Instance.ovr_context = context;

            if (audioSource)
                audioSource.enabled = false;
            if (salsaComponent)
                salsaComponent.enabled = false;
            if (randomEyesComponent)
                randomEyesComponent.enabled = false;

            //.enabled = false;

                //controller
                //JoyStickController.Instance.transform.Find("Movement").gameObject.SetActive(true);
                //JoyStickController.Instance.transform.Find("Rotation").gameObject.SetActive(true);
                //JoyStickController.Instance.SetTarget(transform);
        }
        else {
            characterCamera.SetActive(false);

            if (audioSource)
                audioSource.Play();


//micInput.enabled = false;

//GameState.Instance._networkCharacterList.Add(this);

#if USE_JOYSTICK
            DualStickShooterCharaMotor joyStick = GetComponent<DualStickShooterCharaMotor>();

            joyStick.enabled = false;
#endif

            //if (salsaComponent)
            //    salsaComponent.enabled = false;
            //if (randomEyesComponent)
            //    randomEyesComponent.enabled = false;

            
        }
    }

    


    private void Update() {
        //send lipsync information to stream channel
        //if (m_IsMine) {
        //    if (GameState.Instance.lipSyncSolution == ELipSyncSolution.OculusSolution)
        //    {
        //        sendOculusLipSyncInformation();
        //    }
        //    else if (GameState.Instance.lipSyncSolution == ELipSyncSolution.SalsaSolution)
        //    {
        //        sendSalsaLipsyncInformation();
        //    }
        //}



        //timerToPlayClip -= Time.deltaTime;

        //if (timerToPlayClip <= 0) {
        //    timerToPlayClip = 0.1f;

        //    salsaComponent.Stop();

        //    //play the buffer audio
        //    int samplerate = 44100;

        //    AudioClip clip = AudioClip.Create("clipTemp", samplerate * 2, 1, samplerate, false);

        //    if (audioDataBuffer.Length > 0) {
        //        clip.SetData(audioDataBuffer, 0);
        //        Debug.Log("set data");
        //    }
            

        //    salsaComponent.SetAudioClip(clip);
        //    salsaComponent.Play();
        //}

    }

    private void sendOculusLipSyncInformation() {
        GameState.Instance.sendChannelMsg(JsonUtility.ToJson(context.GetCurrentPhonemeFrame()));

        //use native sdk
        //GameState.Instance.sendChannelMsgNative(JsonUtility.ToJson(context.GetCurrentPhonemeFrame()));
    }

    private void sendSalsaLipsyncInformation() {

        salsaBlendShape.index0 = skinMeshRenderer.GetBlendShapeWeight(0);
        salsaBlendShape.index1 = skinMeshRenderer.GetBlendShapeWeight(1);
        salsaBlendShape.index2 = skinMeshRenderer.GetBlendShapeWeight(2);
        salsaBlendShape.index3 = skinMeshRenderer.GetBlendShapeWeight(3);
        salsaBlendShape.index4 = skinMeshRenderer.GetBlendShapeWeight(4);
        salsaBlendShape.index5 = skinMeshRenderer.GetBlendShapeWeight(5);
        salsaBlendShape.index6 = skinMeshRenderer.GetBlendShapeWeight(6);
        salsaBlendShape.index7 = skinMeshRenderer.GetBlendShapeWeight(7);
        salsaBlendShape.index8 = skinMeshRenderer.GetBlendShapeWeight(8);
        salsaBlendShape.index9 = skinMeshRenderer.GetBlendShapeWeight(9);
        salsaBlendShape.index10 = skinMeshRenderer.GetBlendShapeWeight(10);
        salsaBlendShape.index11 = skinMeshRenderer.GetBlendShapeWeight(11);
        salsaBlendShape.index12 = skinMeshRenderer.GetBlendShapeWeight(12);
        salsaBlendShape.index13 = skinMeshRenderer.GetBlendShapeWeight(13);
        salsaBlendShape.index14 = skinMeshRenderer.GetBlendShapeWeight(14);


        GameState.Instance.sendChannelMsg(JsonUtility.ToJson(salsaBlendShape));

        //use native sdk
        //GameState.Instance.sendChannelMsgNative(JsonUtility.ToJson(salsaBlendShape));
    }

    public void ReceiveLipsyncMessage(string message) {

        //main user should not get this
        if (m_IsMine) return;

        if (GameState.Instance.lipSyncSolution == ELipSyncSolution.OculusSolution)
            receiveOculusLipsynMessage(message);
        else if (GameState.Instance.lipSyncSolution == ELipSyncSolution.SalsaSolution)
            receiveSalsaLipsyncMessage(message);
    }

    public void receiveOculusLipsynMessage(string message)
    {
        OVRLipSync.Frame frame = JsonUtility.FromJson<OVRLipSync.Frame>(message);

        if (frame != null)
        {
            if (currentType == EHeadType.Human)
            {
                morphTarget.SetVisemeToMorphTarget(frame);

                morphTarget.SetLaughterToMorphTarget(frame);
            }
            else
            {
                // Perform smoothing here if on original provider
                if (context.provider == OVRLipSync.ContextProviders.Original)
                {
                    // Go through the current and old
                    for (int i = 0; i < frame.Visemes.Length; i++)
                    {
                        // Convert 1-100 to old * (0.00 - 0.99)
                        float smoothing = ((smoothAmount - 1) / 100.0f);
                        textureFlip.oldFrame.Visemes[i] =
                            textureFlip.oldFrame.Visemes[i] * smoothing +
                            frame.Visemes[i] * (1.0f - smoothing);
                    }
                }
                else
                {
                    textureFlip.oldFrame.Visemes = frame.Visemes;
                }

                textureFlip.SetVisemeToTexture();
            }
        }
    }
    public void receiveSalsaLipsyncMessage(string message) {
        SalsaBlendShape salsaBlendShape = JsonUtility.FromJson<SalsaBlendShape>(message);

        skinMeshRenderer.SetBlendShapeWeight(0, salsaBlendShape.index0);
        skinMeshRenderer.SetBlendShapeWeight(1, salsaBlendShape.index1);
        skinMeshRenderer.SetBlendShapeWeight(2, salsaBlendShape.index2);
        skinMeshRenderer.SetBlendShapeWeight(3, salsaBlendShape.index3);
        skinMeshRenderer.SetBlendShapeWeight(4, salsaBlendShape.index4);
        skinMeshRenderer.SetBlendShapeWeight(5, salsaBlendShape.index5);
        skinMeshRenderer.SetBlendShapeWeight(6, salsaBlendShape.index6);
        skinMeshRenderer.SetBlendShapeWeight(7, salsaBlendShape.index7);
        skinMeshRenderer.SetBlendShapeWeight(8, salsaBlendShape.index8);
        skinMeshRenderer.SetBlendShapeWeight(9, salsaBlendShape.index9);
        skinMeshRenderer.SetBlendShapeWeight(10, salsaBlendShape.index10);
        skinMeshRenderer.SetBlendShapeWeight(11, salsaBlendShape.index11);
        skinMeshRenderer.SetBlendShapeWeight(12, salsaBlendShape.index12);
        skinMeshRenderer.SetBlendShapeWeight(13, salsaBlendShape.index13);
        skinMeshRenderer.SetBlendShapeWeight(14, salsaBlendShape.index14);
    }

#if PHOTON_SOLUTION

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (!photonView.IsMine)
        {
            characterCamera.SetActive(false);
            
            if (audioSource)
                audioSource.enabled = false;

            GameState.Instance._networkCharacterList.Add(this);

            DualStickShooterCharaMotor joyStick = GetComponent<DualStickShooterCharaMotor>();

            joyStick.enabled = false;

            if (salsaComponent)
                salsaComponent.enabled = false;
            if (randomEyesComponent)
                randomEyesComponent.enabled = false;

        } else
        {
            //Transform[] positions = GameManager.Instance.photonManager.transform.GetComponentsInChildren<Transform>();
            //transform.position = positions[Random.Range(0, positions.Length)].position;

            Transform[] childs = GetComponentsInChildren<Transform>();
            foreach (var child in childs) {
                child.gameObject.layer = LayerMask.NameToLayer("SelfOcclude");
            }
            if (audioRenderer)
                audioRenderer.enabled = true;

            GameState.Instance.ovr_context = context;

            //.enabled = false;

            //controller
            //JoyStickController.Instance.transform.Find("Movement").gameObject.SetActive(true);
            //JoyStickController.Instance.transform.Find("Rotation").gameObject.SetActive(true);
            //JoyStickController.Instance.SetTarget(transform);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {



        if (GameState.Instance.lipSyncSolution == ELipSyncSolution.OculusSolution)
        {
            oculusLipSyncSolution(stream, info);
        }
        else if (GameState.Instance.lipSyncSolution == ELipSyncSolution.SalsaSolution) {
            salsaLipSyncSolution(stream, info);
        }

        
    }

    private void oculusLipSyncSolution(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting)
        {
            stream.SendNext((int)currentType);
            stream.SendNext(morphTarget.smoothAmount);
            stream.SendNext(JsonUtility.ToJson(context.GetCurrentPhonemeFrame()));
        }
        else
        {
            object agoraId = photonView.Owner.CustomProperties["agoraId"];
            if (agoraId != null)
                UDID = uint.Parse(agoraId.ToString());

            //synced head
            EHeadType networkHead = (EHeadType)((int)stream.ReceiveNext());
            if (networkHead != currentType)
            {
                currentType = networkHead;
                switchHead.switchHead(currentType);
            }

            //sync smoothing
            smoothAmount = (int)stream.ReceiveNext();
            context.Smoothing = smoothAmount;

            //sync lipsync data
            frameData = (string)stream.ReceiveNext();

            OVRLipSync.Frame frame = JsonUtility.FromJson<OVRLipSync.Frame>(frameData);

            if (frame != null)
            {
                if (currentType == EHeadType.Human)
                {
                    morphTarget.SetVisemeToMorphTarget(frame);

                    morphTarget.SetLaughterToMorphTarget(frame);
                }
                else
                {
                    // Perform smoothing here if on original provider
                    if (context.provider == OVRLipSync.ContextProviders.Original)
                    {
                        // Go through the current and old
                        for (int i = 0; i < frame.Visemes.Length; i++)
                        {
                            // Convert 1-100 to old * (0.00 - 0.99)
                            float smoothing = ((smoothAmount - 1) / 100.0f);
                            textureFlip.oldFrame.Visemes[i] =
                                textureFlip.oldFrame.Visemes[i] * smoothing +
                                frame.Visemes[i] * (1.0f - smoothing);
                        }
                    }
                    else
                    {
                        textureFlip.oldFrame.Visemes = frame.Visemes;
                    }

                    textureFlip.SetVisemeToTexture();
                }
            }

            double pan = GameState.Instance.getPanFromUser(UDID);
            double gain = GameState.Instance.getGainFromUser(UDID);

            //push pan and gain to agora
            if (GameState.Instance.mRtcEngine != null)
                GameState.Instance.mRtcEngine.GetAudioEffectManager().SetRemoteVoicePosition(UDID, pan, gain);


        }
    }

    private void salsaLipSyncSolution(PhotonStream stream, PhotonMessageInfo info) {



        if (stream.IsWriting)
        {

            salsaBlendShape.index0 = skinMeshRenderer.GetBlendShapeWeight(0);
            salsaBlendShape.index1 = skinMeshRenderer.GetBlendShapeWeight(1);
            salsaBlendShape.index2 = skinMeshRenderer.GetBlendShapeWeight(2);
            salsaBlendShape.index3 = skinMeshRenderer.GetBlendShapeWeight(3);
            salsaBlendShape.index4 = skinMeshRenderer.GetBlendShapeWeight(4);
            salsaBlendShape.index5 = skinMeshRenderer.GetBlendShapeWeight(5);
            salsaBlendShape.index6 = skinMeshRenderer.GetBlendShapeWeight(6);
            salsaBlendShape.index7 = skinMeshRenderer.GetBlendShapeWeight(7);
            salsaBlendShape.index8 = skinMeshRenderer.GetBlendShapeWeight(8);
            salsaBlendShape.index9 = skinMeshRenderer.GetBlendShapeWeight(9);
            salsaBlendShape.index10 = skinMeshRenderer.GetBlendShapeWeight(10);
            salsaBlendShape.index11 = skinMeshRenderer.GetBlendShapeWeight(11);
            salsaBlendShape.index12 = skinMeshRenderer.GetBlendShapeWeight(12);
            salsaBlendShape.index13 = skinMeshRenderer.GetBlendShapeWeight(13);
            salsaBlendShape.index14 = skinMeshRenderer.GetBlendShapeWeight(14);


            stream.SendNext(JsonUtility.ToJson(salsaBlendShape));

        }
        else if (stream.IsReading) {

            //sync lipsync data
            frameData = (string)stream.ReceiveNext();

            SalsaBlendShape salsaBlendShape = JsonUtility.FromJson<SalsaBlendShape>(frameData);

            skinMeshRenderer.SetBlendShapeWeight(0, salsaBlendShape.index0);
            skinMeshRenderer.SetBlendShapeWeight(1, salsaBlendShape.index1);
            skinMeshRenderer.SetBlendShapeWeight(2, salsaBlendShape.index2);
            skinMeshRenderer.SetBlendShapeWeight(3, salsaBlendShape.index3);
            skinMeshRenderer.SetBlendShapeWeight(4, salsaBlendShape.index4);
            skinMeshRenderer.SetBlendShapeWeight(5, salsaBlendShape.index5);
            skinMeshRenderer.SetBlendShapeWeight(6, salsaBlendShape.index6);
            skinMeshRenderer.SetBlendShapeWeight(7, salsaBlendShape.index7);
            skinMeshRenderer.SetBlendShapeWeight(8, salsaBlendShape.index8);
            skinMeshRenderer.SetBlendShapeWeight(9, salsaBlendShape.index9);
            skinMeshRenderer.SetBlendShapeWeight(10, salsaBlendShape.index10);
            skinMeshRenderer.SetBlendShapeWeight(11,salsaBlendShape.index11);
            skinMeshRenderer.SetBlendShapeWeight(12, salsaBlendShape.index12);
            skinMeshRenderer.SetBlendShapeWeight(13, salsaBlendShape.index13);
            skinMeshRenderer.SetBlendShapeWeight(14, salsaBlendShape.index14);

        }
    }

#endif

    public void switchMyHead() {
        if (switchHead == null)
            switchHead = GetComponent<SwitchHead>();

        if (currentType == EHeadType.Human) currentType = EHeadType.Robot; else currentType = EHeadType.Human;

        switchHead.switchHead(currentType);
    }
}
