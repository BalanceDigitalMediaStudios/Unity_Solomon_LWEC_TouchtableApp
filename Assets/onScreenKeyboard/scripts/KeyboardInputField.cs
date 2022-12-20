using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;


namespace OnScreenKeyboard{

    [RequireComponent(typeof(TMP_InputField))]
    public class KeyboardInputField : MonoBehaviour, IPointerClickHandler{

        [SerializeField] KeyboardInputFieldHandler handler;


        TMP_InputField inputField;


        void Awake(){ inputField = GetComponent<TMP_InputField>(); }
        public void OnPointerClick(PointerEventData eventData){ handler.BeginInput(inputField); }
    }
}