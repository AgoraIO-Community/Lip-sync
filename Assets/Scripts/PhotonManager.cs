#if PHOTON_SOLUTION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityStandardAssets.Cameras;


public class PhotonManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 3;
        options.EmptyRoomTtl = 0;
        options.PlayerTtl = 0;

        PhotonNetwork.JoinOrCreateRoom("AgoraDemo", options, new TypedLobby("AgoraLobby", LobbyType.Default));
        //PhotonNetwork.CreateRoom("Agora", options, new TypedLobby("AgoraLobby", LobbyType.Default));
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        GameObject obj = null;

        if (PhotonNetwork.CountOfPlayers == 1)
        {
            if (GameState.Instance.lipSyncSolution == ELipSyncSolution.SalsaSolution)
                obj = PhotonNetwork.Instantiate("Ethan", Vector3.zero, Quaternion.identity);
            else if (GameState.Instance.lipSyncSolution == ELipSyncSolution.OculusSolution)
                obj = PhotonNetwork.Instantiate("Character", Vector3.zero, Quaternion.identity);

            obj.transform.SetParent(GameState.Instance.startPoint);
            obj.transform.localPosition = Vector3.zero;
        }
        else if (PhotonNetwork.CountOfPlayers == 2)
        {
            if (GameState.Instance.lipSyncSolution == ELipSyncSolution.SalsaSolution)
                obj = PhotonNetwork.Instantiate("Ethan", Vector3.zero, Quaternion.Euler(0, 180, 0));
            else if (GameState.Instance.lipSyncSolution == ELipSyncSolution.OculusSolution)
                obj = PhotonNetwork.Instantiate("Character", Vector3.zero, Quaternion.Euler(0, 180, 0));

            if (GameState.Instance.lipSyncSolution == ELipSyncSolution.SalsaSolution)
                obj.transform.SetParent(GameState.Instance.startPoint2);
            else if (GameState.Instance.lipSyncSolution == ELipSyncSolution.OculusSolution)
                obj.transform.SetParent(GameState.Instance.startPoint3);
            obj.transform.localPosition = Vector3.zero;
        }
        else {
            if (GameState.Instance.lipSyncSolution == ELipSyncSolution.SalsaSolution)
                obj = PhotonNetwork.Instantiate("Ethan", Vector3.zero, Quaternion.identity);
            else if (GameState.Instance.lipSyncSolution == ELipSyncSolution.OculusSolution)
                obj = PhotonNetwork.Instantiate("Character", Vector3.zero, Quaternion.identity);

            obj.transform.SetParent(GameState.Instance.startPoint);
            obj.transform.localPosition = Helper.getRandomCharacterPos();
        }

      

        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;
        properties.Add("agoraId", GameState.Instance.UDID.ToString());
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

        NetworkCharacter networkChar = obj.GetComponent<NetworkCharacter>();

        if (networkChar) {
            //save ref of main character
            networkChar.UDID = GameState.Instance.UDID;
            GameState.Instance._mainCharacter = networkChar;

            //GameState.Instance.initRecorder();

            GameState.Instance.connectVoiceServer();
        }
            
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        Debug.LogError(message);
    }
}

#endif