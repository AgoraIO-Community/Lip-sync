using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EHeadType {
    Human = 0,
    Robot = 1
}
public class SwitchHead : MonoBehaviour
{
    [SerializeField]
    GameObject humanObject;

    [SerializeField]
    GameObject robotObject;

    [SerializeField]
    OVRLipSyncContextTextureFlip robotComponent;

    [SerializeField]
    OVRLipSyncContextMorphTarget humanCommponent;

    public void switchHead(EHeadType type) {
        switch (type) {
            case EHeadType.Human:
                humanCommponent.enabled = true;
                humanObject.SetActive(true);
                robotComponent.enabled = false;
                robotObject.SetActive(false);

                break;

            case EHeadType.Robot:
                humanCommponent.enabled = false;
                humanObject.SetActive(false);
                robotComponent.enabled = true;
                robotObject.SetActive(true);
                break;
        }
    }
}
