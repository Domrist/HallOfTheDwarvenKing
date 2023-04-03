using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using System;
using System.Threading.Tasks;

public class Dice : MonoBehaviour
{
    /*
    public Button button;
    public Image buttonImage;
    public int oldValue;
    public int valueToBuy;
    public bool markedForBuy;

    public List<Sprite> diceImageListValue;
    public SpriteAtlas diceAtlas;

    public void setDiceImageByValue(int _value){
        buttonImage.sprite = diceAtlas.GetSprite(_value.ToString());
    }
    
    public void Awake(){
        buttonImage = button.GetComponent<Image>();
    }
    
    public IEnumerator rollDieTest(int valu,Action callbackMethod = null){
        int diceValue = valu;
        
        oldValue = valu;
        valueToBuy = valu;
        setDiceImageByValue(valu);
        if(callbackMethod != null){
            callbackMethod();
        }
        yield return new WaitForSeconds(0f); 
    }

    public async void rollDieAsync(float delay,Action callbackMethod = null){
        float timer = 0;
        int diceValue = UnityEngine.Random.Range(1,7);
        while(timer < 1){
            timer = Math.Min(timer + Time.deltaTime/delay,1);
            diceValue = UnityEngine.Random.Range(1,7);
            setDiceImageByValue(diceValue);
            await Task.Yield();
        }
        oldValue = diceValue;
        valueToBuy = diceValue;
        setDiceImageByValue(oldValue);
        if(callbackMethod != null){
            callbackMethod();
        }
    }
    */
}
