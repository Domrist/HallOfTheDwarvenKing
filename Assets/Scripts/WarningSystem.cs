using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public struct AnimationObject{
    public Text textToAnimate;
    public Action callbackBetweenAnimate;
    public AnimationObject(Text t,Action callback){
        this.textToAnimate = t;
        this.callbackBetweenAnimate = callback;
    }
}


public class WarningSystem : MonoBehaviour
{
    public RectTransform rectText;
    public Text text;
    public Image backGroundPanel;
    private Text textToShow;

    private Color zeroColorPanel = new Color(164,164,164,0);
    private Color fullColorPanel = new Color(164,164,164,255);

    private Tween backTween;
    private Tween textTween;    

    public List<AnimationObject> rowDigTextAnimationsList;

    [Header("Timing data for counters")]    
    public float scaleTo;
    public float timeIn;
    public float timeOut;

    public void Awake(){
        DOTween.Init();
        rowDigTextAnimationsList = new List<AnimationObject>();
    }
    
    public void showRoundPhaseMessage(string messageToShow,float t,float tt,Action callbackAction = null){
        backGroundPanel.color = zeroColorPanel;
        backTween.Kill();
        backTween = backGroundPanel.DOFade(1f, t).SetEase(Ease.OutCubic);
        showRoundPhaseText(t,tt,messageToShow,callbackAction); 
    }

    public void showRoundPhaseText(float t,float tt,string _text = null,Action callbackAction = null){
        text.text = _text;
        
        float w = rectText.rect.width;
        rectText.offsetMin = new Vector2(w,0);
        rectText.offsetMax = new Vector2(w,0);
        
        textTween.Kill();
        
        text.DOFade(1f,t/* 2f*/).SetEase(Ease.OutCubic);
        textTween.Kill();
        textTween = rectText.DOAnchorPos(new Vector2(0,0),t,true).OnComplete(()=>{
            backTween.Kill();
            textTween.Kill();
            backTween = backGroundPanel.DOFade(0, tt).SetEase(Ease.InOutQuint);
            float ww = rectText.rect.width;
            rectText.DOAnchorPos(new Vector2(-ww,0),tt,true).SetEase(Ease.InOutQuint).OnComplete(()=>{
                    textTween.Kill();
                    if(callbackAction != null){
                        callbackAction();
                    }
                });
            });
    }

    public void updateCounterWithAnimation(Text t,Action callbackAction){
        t.rectTransform.DOScale(scaleTo,timeIn).OnComplete(()=>{
                if(callbackAction!=null){
                    callbackAction();
                    t.rectTransform.DOScale(1f,timeOut);
                }
            });
    }

    public void fillAndStartSequence(Text textToAppend,Action callbackBetweenAnimation = null,bool isLastElementInSequence = false,Action callbackAtEndOfSequence = null){
        if(!isLastElementInSequence){
            rowDigTextAnimationsList.Add(new AnimationObject(textToAppend,callbackBetweenAnimation));
        }
        if(isLastElementInSequence){
            rowDigTextAnimationsList.Add(new AnimationObject(textToAppend,callbackBetweenAnimation));
            if(callbackAtEndOfSequence!=null){
                callbackAtEndOfSequence();
            }
        }
    }

    public async void resolveAnimationSequenceList(){
        if(rowDigTextAnimationsList.Count != 0){
            foreach(AnimationObject aObject in rowDigTextAnimationsList){
                await Task.Delay(250);
                updateCounterWithAnimation(aObject.textToAnimate,aObject.callbackBetweenAnimate);
            }
        }
        rowDigTextAnimationsList.Clear();
    }
}
