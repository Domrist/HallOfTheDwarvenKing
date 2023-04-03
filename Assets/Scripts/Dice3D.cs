using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.EventSystems;

public class Dice3D : MonoBehaviour
{
    
    public Game game;
    public int id;
    public List<Vector3> sidesTransform;
    private Rigidbody rb;

    public Vector3 diceRotationDirection;

    public float turnSpeed;

    public ForceMode forceMode;

    public int valueToBuy;
    public bool markedForBuy;

    public int oldValue;

    public float testDelay;

    public Action onClick,onClick2;

    public Material diceMaterial;

    public void Awake(){
        sidesTransform = new List<Vector3>(){
            new Vector3(90,270,0),
            new Vector3(90,180,0),
            new Vector3(180,0,0),
            new Vector3(180,180,0),
            new Vector3(90,0,0),
            new Vector3(90,90,0)
        };

        rb = gameObject.GetComponent<Rigidbody>();
        onClick = null;
        onClick2 = null;

    }

   public void OnMouseUpAsButton(){

        if(onClick != null){
            onClick();
        }
        if(onClick2 != null){
            onClick2();
        }
   }

    public async void set3DDiceValue(int _val){
        float speedRotation = 0.25f;
        Action tmp = onClick;
        onClick = null;
        while(Quaternion.Angle(transform.rotation,Quaternion.Euler(sidesTransform[_val-1])) > 0.1f){
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(sidesTransform[_val-1]),speedRotation);
            await Task.Yield();
        }
        onClick = tmp;
        transform.rotation = Quaternion.Euler(sidesTransform[_val-1]);
    }

    public async void roll3DDieAsync(float delay,Action callbackMethod = null){
        float timer = 0;
        int diceValue = UnityEngine.Random.Range(1,7);
        diceRotationDirection = new Vector3(UnityEngine.Random.Range(-20,20),UnityEngine.Random.Range(-20,20),UnityEngine.Random.Range(-20,20));
        rb.AddRelativeTorque(diceRotationDirection * turnSpeed, forceMode);
        while(timer < 1){
            timer = Math.Min(timer + Time.deltaTime/delay,1);
            await Task.Yield();
        }
        oldValue = diceValue;
        valueToBuy = diceValue;
        rb.angularVelocity = Vector3.zero;

        set3DDiceValue(oldValue);
        
        if(callbackMethod != null){
            callbackMethod();
        }
    }

    public void rollDieTest(int valu,Action callbackMethod = null){
        oldValue = valu;
        valueToBuy = valu;
        set3DDiceValue(oldValue);
        if(callbackMethod != null){
            callbackMethod();
        }
    }

    public void reset3DDiceColor(){
        diceMaterial.SetVector("_EmissionColor",new Vector4(0,0,0,0));
    }

    public void setEmissionColor(Color emission){
        diceMaterial.EnableKeyword("_EMISSION");
        diceMaterial.SetVector("_EmissionColor",emission * .35f);   
    }
}