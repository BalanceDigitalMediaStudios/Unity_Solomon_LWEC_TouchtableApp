using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "habitatMatchData", menuName = "Data/Habitat Match Data")]
public class HabitatMatchData : ScriptableObject{

    [SerializeField] int _correctIndex;
    public int correctIndex{get { return _correctIndex; } }

    const int ANSWERCOUNT = 3;
    [SerializeField] HabitatData[] _habitats = new HabitatData[3];
    public HabitatData[] habitats{get { return _habitats; } }    

    [SerializeField] string _flavorText;
    public string flavorText{get { return _flavorText; } }


    void OnValidate(){

        _correctIndex = Mathf.Clamp(_correctIndex, 0, 2);

        if(_habitats.Length != ANSWERCOUNT)
            System.Array.Resize(ref _habitats, ANSWERCOUNT);
    }
}