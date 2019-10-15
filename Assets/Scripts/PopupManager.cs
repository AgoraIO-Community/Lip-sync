using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoSingleton<PopupManager>
{
    [SerializeField]
    CanvasGroup warningPopup;

    [SerializeField]
    Button warningPopupOkBtn;

    [SerializeField]
    CanvasGroup successWarningPopUp;

    [SerializeField]
    Button successWarningPopUpOkBtn;

    [SerializeField]
    CanvasGroup loadingPopUpCanvasGroup;

    public int connectedUser = 0;

    void Start() {
        warningPopupOkBtn.onClick.AddListener(onCloseWarningPopupButtonClicked);

        successWarningPopUpOkBtn.onClick.AddListener(onCloseSuccessWarningPopupButtonClicked);
    }

    public void showLoadingPopUp(bool value) {
        loadingPopUpCanvasGroup.gameObject.SetActive(value);

        if (value)
        {
            loadingPopUpCanvasGroup.interactable = true;
            loadingPopUpCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            loadingPopUpCanvasGroup.interactable = false;
            loadingPopUpCanvasGroup.blocksRaycasts = false;
        }

    }

    // Start is called before the first frame update
    public void showWarningPopup(bool value) {
        warningPopup.gameObject.SetActive(value);

        if (value)
        {
            warningPopup.interactable = true;
            warningPopup.blocksRaycasts = true;
        }
        else {
            warningPopup.interactable = false;
            warningPopup.blocksRaycasts = false;
        }
    }

    public void showSuccessWarningPopup(bool value)
    {
        successWarningPopUp.gameObject.SetActive(value);

        if (value)
        {
            successWarningPopUp.interactable = true;
            successWarningPopUp.blocksRaycasts = true;
        }
        else
        {
            successWarningPopUp.interactable = false;
            successWarningPopUp.blocksRaycasts = false;
        }
    }

    private void onCloseWarningPopupButtonClicked() {
        showWarningPopup(false);
    }

    private void onCloseSuccessWarningPopupButtonClicked()
    {
        showSuccessWarningPopup(false);
    }
}
