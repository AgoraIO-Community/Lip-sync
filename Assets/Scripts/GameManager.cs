using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoSingleton<GameManager>
{
#if PHOTON_SOLUTION
    public PhotonManager photonManager;
#endif

    public event Action<uint, string> onCreateAvatarEvent;
    public event Action<uint, int> onAudioMessageReceivedEvent;
    public event Action<EServerRegion> onAnotherRegionSelectedEvent;

    public event Action<uint> onUserLeaveChannelEvent;

    public EServerRegion currentRegion = EServerRegion.Auto;

    public float maxDistanceToHear = 200;

    private string appId = "4133ccb0f5c5455a98d8d2a9ef00dc4b";

    // Start is called before the first frame update
    void Start()
    {

  

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void enableAudioObserver() {


    }

    public void createAvatar(uint udid, string name)
    {
        if (onCreateAvatarEvent != null)
            onCreateAvatarEvent(udid, name);
    }

    public void MessageReceived(uint udid, int index) {
        if (onAudioMessageReceivedEvent != null)
            onAudioMessageReceivedEvent(udid, index);
    }

    public void UserLeaveChannel(uint udid) {
        if (onUserLeaveChannelEvent != null)
            onUserLeaveChannelEvent(udid);
    }

    public void anotherRegionClicked(EServerRegion region) {
        if (onAnotherRegionSelectedEvent != null)
            onAnotherRegionSelectedEvent(region);
    }
}
