#if PHOTON_SOLUTION
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionCreator : MonoBehaviour
{
    [SerializeField]
    GameObject regionPrefab;

    List<PhotonRegion> regionList = new List<PhotonRegion>();

    public void initRegion(EServerRegion region, bool isCheck) {
        GameObject obj = Instantiate(regionPrefab, transform);

        PhotonRegion newRegion = obj.GetComponent<PhotonRegion>();

        newRegion.init(region, isCheck);

        regionList.Add(newRegion);
    }
}
#endif