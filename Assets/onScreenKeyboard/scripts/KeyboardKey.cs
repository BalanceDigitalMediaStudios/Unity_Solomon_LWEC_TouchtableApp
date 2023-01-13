using UnityEngine;
using TMPro;
using UnityEngine.UI;



namespace OnScreenKeyboard{
    
    public class KeyboardKey : MonoBehaviour{

        [Header("Values")]
        public string lowerValue;
        public string upperValue;
        
        string      value;
        new string  tag;

        [Header("UI")]
        [SerializeField] TextMeshProUGUI    lowerText;
        [SerializeField] TextMeshProUGUI    upperText;

        [Header("Colors")]
        [SerializeField] Color lowerColorActive     = Color.black;
        [SerializeField] Color lowerColorInactive   = Color.grey;
        [SerializeField] Color upperColorActive     = Color.black;
        [SerializeField] Color upperColorInactive   = Color.grey;

        Keyboard    keyboard;
        Button      button;


        public void Initialize(Keyboard kb){

            //get components
            keyboard = kb;

            SetInitialText();

            //add event listeners
            button = GetComponent<Button>();
            if(button) button.onClick.AddListener(PressKey);

            keyboard.onSetCase += OnSetCase;
        }

        void OnDestroy(){

            if(!keyboard)
                return;
                
            keyboard.onSetCase -= OnSetCase;
        }

        public void PressKey(){

            if(keyboard != null)
                keyboard.PressKey(value, tag);
        }



        void OnSetCase(bool isUpper){

            string input    = isUpper ? upperValue : lowerValue;
            tag             = Keyboard.GetTag(input);
            value           = Keyboard.RemoveTag(input).Replace("\\n","\n");

            //if we want to show both upper and lower text objects
            if(lowerText && upperText && upperText.gameObject.activeSelf)
            {
                lowerText.color = !isUpper  ? lowerColorActive : lowerColorInactive;
                upperText.color = isUpper   ? upperColorActive : upperColorInactive;
            }
            //if we want to swap upper and lower on a single text object
            else if(lowerText)
                lowerText.text = value;
        }

        void OnValidate(){

            SetInitialText();
        }

        void SetInitialText(){

            if (lowerText) lowerText.text = Keyboard.RemoveTag(lowerValue).Replace("\\n","\n");
            if (upperText) upperText.text = Keyboard.RemoveTag(upperValue).Replace("\\n","\n");
        }
    }
}