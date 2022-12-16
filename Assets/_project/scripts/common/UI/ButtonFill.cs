using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonFill : MonoBehaviour{

    public Image fillImage;

    void OnEnable(){

        fillImage.fillAmount = 0;
    }


    public void SetFill(float value){

        fillImage.fillAmount = value;
    }
}