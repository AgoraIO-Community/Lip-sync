#if PHOTON_SOLUTION
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonRegion : MonoBehaviour
{
    [SerializeField]
    Button button;

    [SerializeField]
    GameObject uncheck;

    [SerializeField]
    GameObject check;

    [SerializeField]
    Text regionTxt;

    EServerRegion _region = EServerRegion.Auto;


    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(onButtonClicked);

        GameManager.Instance.onAnotherRegionSelectedEvent += onAnotherButtonClicked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy() {
        GameManager.Instance.onAnotherRegionSelectedEvent -= onAnotherButtonClicked;
    }

    public void init(EServerRegion region, bool isCheck) {
        regionTxt.text = region.ToString();

        check.SetActive(isCheck);
        uncheck.SetActive(!isCheck);

        _region = region;
    }

    private void onButtonClicked()
    {
        Debug.Log("region button clicked");

        GameManager.Instance.currentRegion = _region;

        check.SetActive(true);
        uncheck.SetActive(false);

        GameManager.Instance.anotherRegionClicked(_region);
    }

    private void onAnotherButtonClicked(EServerRegion region) {
        if (region != _region) {
            check.SetActive(false);
            uncheck.SetActive(true);
        }
    }
}
#endif