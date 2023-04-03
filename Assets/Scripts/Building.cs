using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    public int depthLevel;
    public BuildingProperties props;
    public Action MethodBeforeBuild;
    public Action MethodAfterBuild;
    public Action MiddleAction;
    
    public bool setted;

    public void onClickDefaultMethod(){
        if(MethodBeforeBuild != null){
            MethodBeforeBuild();
        }
        if(MiddleAction!=null){
            MiddleAction();
        }
    }
    public Button button;
    public Cell parentCell;
    public bool isBuildingUnique;
    public bool isBuildedAlready;

    public void Awake(){
        parentCell = null;
        setted = false;
        MethodBeforeBuild = null;
        MethodAfterBuild = null;
        MiddleAction = null;
        button.onClick.AddListener( ()=>{onClickDefaultMethod();});
    }

}