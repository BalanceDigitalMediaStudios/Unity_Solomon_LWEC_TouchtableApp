using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;


namespace OnScreenKeyboard{

    public class Keyboard : MonoBehaviour{

        public delegate void    KeyboardBoolEvent (bool value);
        public event            KeyboardBoolEvent onSetCase;
        public event            KeyboardBoolEvent onSetShift;
        public event            KeyboardBoolEvent onSetCaps;

        public delegate void    KeyboardPressEvent (string value, string tag);
        public event            KeyboardPressEvent onPressKey;

        [System.Serializable]
        private class KeyboardLayout{

            public string       id;
            public GameObject   group;
        }


        [SerializeField] KeyboardLayout[]           layouts;
        [SerializeField, ReadOnly] KeyboardLayout   currentLayout;

        [Space(10)]
        [SerializeField, ReadOnly] string currentValue;
        [SerializeField, ReadOnly] string currentTag;
        [SerializeField, ReadOnly] bool isShift;
        [SerializeField, ReadOnly] bool isCaps;
        [SerializeField, ReadOnly] bool _isUpper;
        bool isUpper{
            get { return _isUpper; }
            set { _isUpper = value; if(onSetCase != null) onSetCase(_isUpper); }
        }
        [SerializeField, ReadOnly] GameObject lastSelection;
        
        


        static Regex regex = new Regex(@"<[\w]+>");

        

        /*void Awake(){

            Application.quitting += ClearEventListeners;
        }        
        void ClearEventListeners(){

            onSetCase   = null;
            onSetShift  = null;
            onSetCaps   = null;            
        }*/


        void Start(){ Initialize(); }
        void Initialize(){

            //get all keys and initialize them
            KeyboardKey[] keys = GetComponentsInChildren<KeyboardKey>(true);
            foreach(KeyboardKey k in GetComponentsInChildren<KeyboardKey>(true))
                k.Initialize(this);
            
            //get all toggle keys and initialize them
            ToggleKey[] toggles = GetComponentsInChildren<ToggleKey>(true);
            foreach(ToggleKey t in toggles)
                t.Initialize(this);


            isShift = false;
            isCaps  = false;
            UpdateCase();
            SetLayout(layouts[0].id);

            //store last non-keyboard key selection
            lastSelection = GetCurrentSelectedGameObject();
        }

        

        public GameObject GetCurrentSelectedGameObject(){

            return EventSystem.current.currentSelectedGameObject;
        }
        public void SetSelectedGameObject(GameObject go){

            EventSystem.current.SetSelectedGameObject(go);
        }

        void Update(){

            //check if selection has changed
            if(lastSelection != EventSystem.current.currentSelectedGameObject)
            {
                //cancel new selection if part of keyboard and not return key
                if(GetCurrentSelectedGameObject() != null && GetCurrentSelectedGameObject().GetComponentInParent<Keyboard>() != null && currentTag != "RETURN")
                    SetSelectedGameObject(lastSelection);
                
                //otherwise, update selection
                else
                    lastSelection = GetCurrentSelectedGameObject();
            } 
        }


        public static string RemoveTag(string input){

            string output = input;

            Match m = regex.Match(input);
            if(m.Success)
            {
                //Debug.LogFormat("Removed Tag: {0}", m.Value.Replace("<","").Replace(">",""));
                output = output.Replace(m.Value, "");
            }
            return output;
        }
        public static string GetTag(string input){

            List<string> output = new List<string>(0);

            Match m = regex.Match(input);
            if(m.Success)
            {
                //Debug.LogFormat("Found Tag: {0}", m.Value.Replace("<","").Replace(">",""));
                return m.Value.Replace("<","").Replace(">","");
            }
            return "";
        }



        public void PressKey(string value, string tag){            

            //check for tags
            switch(tag)
            {         
                case "SHIFT":
                    SetShift(!isShift);
                    break;
                
                case "CAPSLOCK":
                    SetCapsLock(!isCaps);
                    break;
                
                case "SPACE":
                    value   = " ";
                    tag     = "STRING";
                    break;
                
                case "":
                    tag     = "STRING";
                    break;

                case string a when a.Contains("LAYOUT_"):
                    SetLayout(a.Replace("LAYOUT_", ""));
                    break;
            }

            //send press event
            //Debug.LogFormat("Pressed Key: [{0}] Tag: <{1}>", value, tag);
            currentValue    = value;
            currentTag      = tag;
            if(onPressKey != null)
                onPressKey(value, tag);
            
            //get rid of shift-state after specific types of input
            if(isShift && tag == "STRING")
                SetShift(false);
        }

        public void SetShift(bool shift){

            isShift = shift;
            UpdateCase();

            if(onSetShift != null)
                onSetShift(isShift);
        }
        public void SetCapsLock(bool caps){

            isCaps = caps;
            UpdateCase();

            if(onSetCaps != null)
                onSetCaps(isCaps);
        }
        void UpdateCase(){ isUpper = (isShift && !isCaps || !isShift && isCaps); }


        public void SetLayout(string id){

            foreach(KeyboardLayout l in layouts)
                l.group.SetActive(l.id == id);
        }
    }
}