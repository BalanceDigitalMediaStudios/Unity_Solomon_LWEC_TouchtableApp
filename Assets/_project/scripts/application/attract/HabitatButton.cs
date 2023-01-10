using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HabitatButton : ZonedMonobehaviour{

    public delegate void  ButtonEvent (string zoneId, HabitatButton button);
    public static   event ButtonEvent onSelectHabitat;


    [SerializeField] HabitatData _data;
    public HabitatData data{get { return _data; } }
    
    [SerializeField] Image _image;
    Image image{ get{ return _image; } }


    Button _button;
    public Button button{ get{ if(_button == null) _button = GetComponent<Button>(); return _button; } }
    


    void Awake(){ button.onClick.AddListener(ButtonAction); }
    void ButtonAction(){

        if(onSelectHabitat != null)
            onSelectHabitat(zoneId, this);
    }



    void OnValidate(){

        //automatically set button image according to habitat data
        if(data != null && data.buttonSprite != null && image != null)
            image.sprite = data.buttonSprite;
    }
}