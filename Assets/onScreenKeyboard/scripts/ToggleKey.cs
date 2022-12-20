using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OnScreenKeyboard{

    public class ToggleKey : MonoBehaviour{

        public enum SpecialKey { shift, capsLock };

        [SerializeField] new SpecialKey     tag;
        [SerializeField] Image              bg;
        [SerializeField] TextMeshProUGUI    text;
        [SerializeField] Color              onColorBg   = Color.white;
        [SerializeField] Color              onColorText = Color.white;

        Keyboard keyboard;
        Color bgColor, textColor;

        public void Initialize(Keyboard kb){

            //get components
            keyboard = kb;

            //store initial colors
            if(bg)      bgColor     = bg.color;
            if(text)    textColor   = text.color;

            //add event listeners
            switch(tag)
            {
                case SpecialKey.shift:
                    keyboard.onSetShift += OnSetHold;
                    break;
                case SpecialKey.capsLock:
                    keyboard.onSetCaps += OnSetHold;
                    break;
            }
        }

        void OnDestroy(){

            //add event listeners
            switch(tag)
            {
                case SpecialKey.shift:
                    keyboard.onSetShift -= OnSetHold;
                    break;
                case SpecialKey.capsLock:
                    keyboard.onSetCaps -= OnSetHold;
                    break;
            }
        }

        void OnSetHold(bool hold){

            if(bg)      bg.color    = hold ? onColorBg    : bgColor;
            if(text)    text.color  = hold ? onColorText  : textColor;
        }
    }
}