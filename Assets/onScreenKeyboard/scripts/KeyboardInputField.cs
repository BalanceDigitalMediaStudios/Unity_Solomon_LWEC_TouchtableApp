using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


namespace OnScreenKeyboard{

    [RequireComponent(typeof(TMP_InputField))]
    public class KeyboardInputField : MonoBehaviour, IPointerClickHandler{

        [SerializeField] KeyboardInputFieldHandler handler;


        TMP_InputField _inputField;
        public TMP_InputField inputField{ get { if(_inputField == null) _inputField = GetComponent<TMP_InputField>(); return _inputField; } }

        Color caretColor;


        void Awake(){

            caretColor = inputField.caretColor;
        }
        public void OnPointerClick(PointerEventData eventData){ 
            
            handler.BeginInput(inputField);
            inputField.caretColor = caretColor;
        }
    }
}