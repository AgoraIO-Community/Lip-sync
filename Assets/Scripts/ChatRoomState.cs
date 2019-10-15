using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatRoomState : BaseState
{
    //[SerializeField]
    //private AvatarCreator avatarCreator;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnEnter()
    {
        base.OnEnter();

        GameManager.Instance.onCreateAvatarEvent += createAvatar;
        GameManager.Instance.onUserLeaveChannelEvent += onUserLeave;
    }

    //set up your own video
    private void createAvatar(uint uid, string name) {
        //avatarCreator.initLipSyncPrefab(uid, name);
    }

    public override void OnExit()
    {
        base.OnExit();

        GameManager.Instance.onCreateAvatarEvent -= createAvatar;
        GameManager.Instance.onUserLeaveChannelEvent -= onUserLeave;
    }

    public void onBackButtonClicked() {
        GameState.Instance.mRtcEngine.LeaveChannel();
        //avatarCreator.cleanAllAvatar();
        GameState.Instance.PopState();
    }

    private void onUserLeave(uint udid) {
        //avatarCreator.removeUser(udid);
    }

    public void onSwitchHeadButtonClicked() {
        Debug.Log("onSwitchHeadButtonClicked");
        GameState.Instance._mainCharacter.switchMyHead();
    }
}
