using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;

public enum Directions{
    Right,
    Left,
    Up,
    Down,
    PopUp
}

public class UIPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public bool hide;
    public RectTransform rect;
    public Directions hideDirection;
    public static List<UIPanel> panels = new List<UIPanel>();

    public string panelName;

    private float h;

    private float timeInterpolationForRect = 0;
    public float interPolationSpeed;

    public void Awake(){
        panels.Add(this);        
        h = rect.rect.height;
    }

    public static void showNamedPanel(string _panelToShow = null){
        if(_panelToShow != null){
            foreach(UIPanel panel in panels){
                if(panel != null){ //от греха подальше
                    if(panel.panelName == _panelToShow){
                        panel.hide = false;
                        panel.moveToAsync(new Vector2(0,0));
                    }
                    else{
                        panel.hidePanel();    
                    }
                }
            }
        }
    }

    public void showPanel(string _panelToShow = null){
        if(_panelToShow != null){
            foreach(UIPanel panel in panels){
                if(panel != null){ //от греха подальше
                    if(panel.panelName == _panelToShow){
                        panel.hide = false;
                        StartCoroutine(panel.moveTo(new Vector2(0,0)));
                    }
                    else{
                        panel.hidePanel();    
                    }
                }
            }
        }
        else{
            hide = false;
            StartCoroutine(moveTo(new Vector2(0,0)));
            foreach(UIPanel panel in panels){
                if(panel != this && panel != null){
                    panel.hidePanel();
                }
            }
        }
    }

    IEnumerator moveTo(Vector2 destinationVector){ //вот эту хуйню потом переделать под async
        timeInterpolationForRect = 0;
        while( 0.01f <= Vector2.Distance(rect.offsetMin, destinationVector) && 0.01f <= Vector2.Distance(rect.offsetMax, destinationVector)){
            rect.offsetMin = Vector2.Lerp(rect.offsetMin,destinationVector,timeInterpolationForRect);
            rect.offsetMax = Vector2.Lerp(rect.offsetMax,destinationVector,timeInterpolationForRect);
            timeInterpolationForRect += interPolationSpeed;
            yield return null;
        }
        rect.offsetMin = destinationVector;
        rect.offsetMax = destinationVector;
    } 

    public async void moveToAsync(Vector2 destinationVector){
        timeInterpolationForRect = 0;
        
        while( 0.01f <= Vector2.Distance(rect.offsetMin, destinationVector) && 0.01f <= Vector2.Distance(rect.offsetMax, destinationVector)){
            rect.offsetMin = Vector2.Lerp(rect.offsetMin,destinationVector,timeInterpolationForRect);
            rect.offsetMax = Vector2.Lerp(rect.offsetMax,destinationVector,timeInterpolationForRect);
            timeInterpolationForRect += interPolationSpeed;
            await Task.Yield();
        }

        rect.offsetMin = destinationVector;
        rect.offsetMax = destinationVector;        
    }

    public static void hideNamedPanel(string _panelToHide){
        foreach(UIPanel panel in UIPanel.panels){
            if(panel != null && panel.panelName == _panelToHide){
                panel.hidePanel();
            }
        }
    }

    public void hidePanel(){
        if(this.hide){
            return;
        }
        hide = true;

        float w = Screen.width;

        switch(hideDirection){
            case Directions.Down:
                StartCoroutine(moveTo(new Vector2(0,-h)));
                break;
            case Directions.Right:
                StartCoroutine(moveTo(new Vector2(w,0)));
                break;
            case Directions.Left:
                StartCoroutine(moveTo(new Vector2(-w,0)));
                break;
        }
    }

    public void showHide(){
        if(hide){
            showPanel();
            return;
        }
        else if(!hide){
            hidePanel();
            return;
        }
    }
}