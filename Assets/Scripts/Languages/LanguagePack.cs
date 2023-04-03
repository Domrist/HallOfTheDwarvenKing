using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Dict{
    public string key;
    public string val;
}

[CreateAssetMenu(fileName="Building",menuName="HallOfTheDwarvenKing/Create new language pack",order=51)]
public class LanguagePack : ScriptableObject
{
    // Start is called before the first frame update
    public string nextButtonString;
    public string cancelButtonString;
    public List<Dict> mainButtonsValues;
    public List<Dict> phaseName;
    public List<BuildingDescriptionLanguage> shopBuildingLocalisationData;
    
    public string getValueForButton(string val){
        foreach(Dict d in mainButtonsValues){
            if(d.key == val){
                return d.val;
            }
        }
        return "\0";
    }

    public string getPhaseNameByKey(string keyToFind){
        foreach(Dict d in phaseName){
            if(d.key == keyToFind){
                return d.val;
            }
        }
        return "\0";   
    }
}
