using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OnScreenKeyboard{

    public class KeyboardInputFieldHandler : MonoBehaviour{

        [SerializeField] Keyboard keyboard;
        [SerializeField] Button[] endInputButtons;

        [Header("Debug")]
        [SerializeField, ReadOnly] TMP_InputField   inputField;
        [SerializeField, ReadOnly] string 	        currentText     = string.Empty;
	    [SerializeField, ReadOnly] int 	            caretPostion 	= -1;

        void Awake()    { foreach(Button b in endInputButtons) b.onClick.AddListener(EndInput); }
        void OnEnable() { keyboard.onPressKey += OnPressKey; }
        void OnDisable(){ keyboard.onPressKey -= OnPressKey; }
        void Update(){

            if(inputField)
                caretPostion = inputField.caretPosition;
        }


        public void BeginInput(TMP_InputField field){

            inputField = field;
            if(!inputField)
                return;

            currentText                 = inputField.text;
            caretPostion                = currentText.Length;
            inputField.caretColor       = Color.black;
        }

        public void EndInput(){

            if(!inputField)
                return;

            inputField.text             = currentText;
            inputField.caretPosition    = 0;
            inputField.caretColor       = Color.clear;
            inputField.onEndEdit.Invoke(inputField.gameObject.name);

            inputField      = null;
            currentText     = "";
            caretPostion    = -1;
        }


        void OnPressKey(string value, string tag){

            if(!inputField)
                return;

            //check for tags
            switch(tag)
            {   
                case "RETURN":
                    EndInput();
                    return;

                case "NEWLINE":
                    NewLine();
                    break;

                case "BACKSPACE":
                    Backspace();
                    break;
                
                case "DELETE":
                    Delete();
                    break; 
                
                case "CLEAR":
                    Clear();
                    break;
                
                case "STRING":
                    Add(value);
                    break;
            }

            UpdateInputField();
        }

        void UpdateInputField(){

            inputField.Select();        //keyboard UI buttons will naturally take back selection, so we need to reselect the inputfield so the caret is visible
            inputField.text             = currentText;
            inputField.caretPosition    = caretPostion;
            inputField.caretColor       = Color.black;
        }

        void Backspace(){

            int selectAnchorPositon = inputField.selectionAnchorPosition;
            int selectFocusPositon 	= inputField.selectionFocusPosition;

            int selectionStart 	= Mathf.Min(selectAnchorPositon, selectFocusPositon);
            int selectionEnd 	= Mathf.Max(selectAnchorPositon, selectFocusPositon);

            //no selection means remove a character at caret (as long as not at first position)
            if(selectionStart == selectionEnd && caretPostion != 0)
            {
                currentText = currentText.Substring(0, caretPostion - 1) + currentText.Substring(caretPostion, currentText.Length - caretPostion);
                caretPostion--;
            }
            //selection mean remove only what is selected
            else
            {
                currentText = currentText.Substring(0, selectionStart) + currentText.Substring(selectionEnd, currentText.Length - selectionEnd);
                caretPostion = selectionStart;
            }
        }
        void Delete(){
        }
        void NewLine(){
        }
        void Clear(){

            currentText     = "";
            caretPostion    = 0;
        }
        

        void Add(string s){

            int selectAnchorPositon = inputField.selectionAnchorPosition;
            int selectFocusPositon 	= inputField.selectionFocusPosition;

            int selectionStart 	= Mathf.Min(selectAnchorPositon, selectFocusPositon);
            int selectionEnd 	= Mathf.Max(selectAnchorPositon, selectFocusPositon);

            //no selection means insert string at caret position
            if(selectionStart == selectionEnd)
            {
                currentText = currentText.Substring(0, caretPostion) + s + currentText.Substring(caretPostion, currentText.Length - caretPostion);	
                caretPostion += s.Length;
            }
            //selection mean replace what is selected with new string
            else
            {
                currentText = currentText.Substring(0, selectionStart) + s + currentText.Substring(selectionEnd, currentText.Length - selectionEnd);
                caretPostion = selectionStart + s.Length;
            }
        }
    }
}