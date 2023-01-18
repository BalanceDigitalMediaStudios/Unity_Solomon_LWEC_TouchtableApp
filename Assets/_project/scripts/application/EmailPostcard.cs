using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using OnScreenKeyboard;


public class EmailPostcard : MonoBehaviour{    

    [Header("Upload")]
    [SerializeField] string url;
    [SerializeField] string token;
    [SerializeField] string filePrefix = "snapshots\\filePrefix";
    [SerializeField] float  minimumLoadingTime;

    [Header("Snapshot")]    
    [SerializeField] Camera                 snapshotCamera;
    [SerializeField] RectTransform          snapshotRect;
    [Tooltip("Desired width of the image.  NOTE: This is used BEFORE image rotation, so images that are sideways should enter desired height instead")]
    [SerializeField] int                    snapshotWidth;
    [Tooltip("After snapshot is taken, how should the image be rotated to orient correctly?")]
    [SerializeField] SnapshotMaker.Rotation correctiveRotation = SnapshotMaker.Rotation.none;
    [SerializeField, ReadOnly, TexturePreview(200)] Texture2D outputTexture;

    [Header("Main")]
    [SerializeField] UITransitionFade mainFade;

    [Header("Enter Email Screen")]
    [SerializeField] UITransitionFade   entryFade;
    [SerializeField] TMP_InputField     inputField;
    [SerializeField] Button             cancelButton, confirmButton;
    [SerializeField] UITransitionFade   confirmPreLoadFade, confirmPostLoadFade;

    [Header("Keyboard")]
    [SerializeField] KeyboardInputFieldHandler  handler;
    [SerializeField] Keyboard                   keyboard;
    [SerializeField] CanvasGroup                keyboardCG;

    [Header("Results Screen")]
    [SerializeField] UITransitionFade   resultsFade;
    [SerializeField] TextMeshProUGUI    resultTitleText, resultDescriptionText;
    [SerializeField] TMP_TypeOutEffect  resultMessageTyping;
    [SerializeField] Button             continueButton;


    //regex to validate an email address
    const string emailRegex =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";



    void Awake(){

        inputField.     onValueChanged. AddListener(OnUpdateInputField);
        cancelButton.   onClick.        AddListener(Close);
        confirmButton.  onClick.        AddListener(ConfirmButton);
        continueButton. onClick.        AddListener(Close);
    }
    void OnEnable() { resultMessageTyping.onFinish.AddListener(OnResultMessageTyped); }
    void OnDisable(){ resultMessageTyping.onFinish.RemoveListener(OnResultMessageTyped); }



    public void Open(){

        SnapshotMaker.instance.TakeSnapshot(snapshotCamera, SnapshotMaker.RectTransformToScreenSpaceRect(snapshotCamera, snapshotRect), snapshotWidth, (texture) =>
        {
            mainFade.canvasGroup.alpha = 0;
            mainFade.gameObject.SetActive(true);
            mainFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.always;            

            StopAllCoroutines();
            StartCoroutine(OpenRoutine(texture));
        });
    }
    IEnumerator OpenRoutine(Texture2D texture){

        //store texture with correct rotation
        texture.name = "tempPostcard";
        outputTexture = SnapshotMaker.RotateTexture(texture, correctiveRotation);
        yield return new WaitForEndOfFrame();

        //fade in menu, enable entry screen and disable results screen
        mainFade.TransitionToEnd(false);
        entryFade.gameObject.SetActive(true);
        entryFade.canvasGroup.interactable = true;
        resultsFade.gameObject.SetActive(false);

        //enable preload message for confirm button
        confirmPreLoadFade. gameObject.SetActive(true);
        confirmPostLoadFade.gameObject.SetActive(false);

        //reset inputfield
        confirmPreLoadFade.canvasGroup.alpha = 1;
        inputField.text = string.Empty;
        OnUpdateInputField(string.Empty);

        //reset and enable keyboard
        keyboardCG.blocksRaycasts = true;
        keyboard.SetShift(false);
        keyboard.SetCapsLock(false);

        //select input field
        handler.BeginInput(inputField);
    }
    void Close(){

        mainFade.blockRaycastCondition = UITransitionFade.BlockRaycastCondition.never;
        mainFade.TransitionToStart(true);
    }


    void OnUpdateInputField(string text){

        confirmButton.interactable = IsEmail(text);
    }       
    bool IsEmail(string email){

        if (email != null) return Regex.IsMatch(email, emailRegex);
        else return false;
    }


    void ConfirmButton(){

        //begin upload
        UploadTexture(outputTexture, url, OnSuccess, OnFail);

        //disable menu interaction
        entryFade.canvasGroup.interactable      = false;
        entryFade.canvasGroup.blocksRaycasts    = false;
        keyboardCG.blocksRaycasts               = false;

        //show loading text
        confirmPreLoadFade.TransitionToStart(true);
        confirmPostLoadFade.gameObject.SetActive(true);
    }


    void UploadTexture(Texture2D tex, string url, System.Action<string> onSuccess, System.Action<string> onFail){

        byte[] bytes = tex.EncodeToJPG(100);
        string fileName = string.Format("{0}_{1}.{2}", filePrefix, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"), "jpg");
        
        WWWForm form = new WWWForm();
        form.AddField("token", token);
        form.AddField("email", inputField.text);
        form.AddBinaryData("uploadedFile", bytes, fileName, "image/jpg");

        Debug.LogFormat("Form upload parameters:\n{0}{1}\n{2}{3}\n{4}{5}",
            string.Format("   {0, -14}", "token:"),         FormUploader.Colorize(token, "yellow"),
            string.Format("   {0, -14}", "email:"),         FormUploader.Colorize(inputField.text, "yellow"),
            string.Format("   {0, -14}", "uploadedFile:"),  FormUploader.Colorize(string.Format("{0} bytes", bytes.Length.ToString()), "yellow"));
        FormUploader.instance.Upload(form, url, minimumLoadingTime, onSuccess, onFail);
    }
    void OnSuccess(string response){

        //set text and open results screen
        resultTitleText.text        = "POSTCARD SENT!";
        resultDescriptionText.text  = "Thank you for visiting!";
        ShowResults();
    }
    void OnFail(string error){
        
        //set text and open results screen
        resultTitleText.text        = "SENDING FAILED";
        resultDescriptionText.text  = "There was a problem sending the postcard.\nPlease check your network connection and try again.";
        ShowResults();
    }

    void ShowResults(){

        //fade out entry screen and fade in results
        entryFade.TransitionToStart(true);
        resultsFade.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
    }

    void OnResultMessageTyped(){

        continueButton.gameObject.SetActive(true);
    }
}