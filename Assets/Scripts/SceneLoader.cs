using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Threading.Tasks;
using System;

public class SceneLoader : MonoBehaviour
{
    public RectTransform hideShowPanel;
    public RectTransform infoPanel;

    public string sceneNameToLoad;

    public float sceneInPanelUpSpeed;

    public Game gameInstance;

    private float interPolationSpeed = 0.065f;

    public void Awake(){
        Application.targetFrameRate = 60;
        hideShowPanel.anchorMax = new Vector2(1,1);
        hideShowPanel.anchorMin = new Vector2(0,0);
        float h = hideShowPanel.rect.height;
        hideShowPanel.offsetMax = new Vector2(0,0);
        hideShowPanel.offsetMin = new Vector2(0,0);

        if(SceneManager.GetActiveScene().name == "MainMenu"){
            hideShowPanel.DOAnchorMin(new Vector2(0,1),sceneInPanelUpSpeed).SetEase(Ease.Linear);
            float hh = Screen.height;
            infoPanel.offsetMin = new Vector2(0, -hh);
            infoPanel.offsetMax = new Vector2(0, -hh);
        }
        if(SceneManager.GetActiveScene().name == "GameScene"){
            if(gameInstance != null){
                hideShowPanel.DOAnchorMin(new Vector2(0,1),sceneInPanelUpSpeed).SetEase(Ease.Linear);
            }
        }
    }

    public async void showAboutGamePanel(){
        float timeInterpolationForRect = 0;
        while( 0.01f <= Vector2.Distance(infoPanel.offsetMin, Vector2.zero) && 0.01f <= Vector2.Distance(infoPanel.offsetMax, Vector2.zero)){
            infoPanel.offsetMin = Vector2.Lerp(infoPanel.offsetMin,Vector2.zero,timeInterpolationForRect);
            infoPanel.offsetMax = Vector2.Lerp(infoPanel.offsetMax,Vector2.zero,timeInterpolationForRect);
            timeInterpolationForRect += interPolationSpeed;
            await Task.Yield();            
        }
        infoPanel.offsetMin = Vector2.zero;
        infoPanel.offsetMax = Vector2.zero;
    }

    public async void hideAboutGamePanel(){
        float timeInterpolationForRect = 0;
        Vector2 destination = new Vector2(0,-Screen.height);
        while( 0.01f <= Vector2.Distance(infoPanel.offsetMin, destination) && 0.01f <= Vector2.Distance(infoPanel.offsetMax, destination)){
            infoPanel.offsetMin = Vector2.Lerp(infoPanel.offsetMin,destination,timeInterpolationForRect);
            infoPanel.offsetMax = Vector2.Lerp(infoPanel.offsetMax,destination,timeInterpolationForRect);
            timeInterpolationForRect += interPolationSpeed;
            await Task.Yield();            
        }
        infoPanel.offsetMin = destination;
        infoPanel.offsetMax = destination;   
    }

    public void loadScene(){ //поднимаем из под низа экрана шторку и закрываем экран для смена сцены
        
        hideShowPanel.anchorMax = new Vector2(1,1);
        hideShowPanel.anchorMin = new Vector2(0,0);
        float h = hideShowPanel.rect.height;
        hideShowPanel.offsetMax = new Vector2(0,-h);
        hideShowPanel.offsetMin = new Vector2(0,-h);
        hideShowPanel.DOAnchorPos(new Vector2(0,0),.5f,true).SetEase(Ease.Linear).OnComplete(()=>{SceneManager.LoadScene(sceneNameToLoad);});
    }
}
