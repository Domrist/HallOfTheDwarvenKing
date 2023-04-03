using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepthRow : MonoBehaviour
{
    public int countOfDigToAccess;
    public int currentCountOfDigToAccess;
    public List<int> gemLevelAccessList;
    public List<int> dynamicGemLevelAccessList;
    public bool accessToRowAllowed;
    public List<Cell> cellList;
    
    public Text rowText;

    public void Awake(){
        rowText.text = countOfDigToAccess.ToString();
    }

    public int getCountOfDiggedGems(){
        int gems = 0;
        List<int> trashCan = new List<int>();
        foreach(int i in dynamicGemLevelAccessList){
            if(i <= currentCountOfDigToAccess){
                trashCan.Add(i);
                gems++;
            }
        }
        foreach(int i in trashCan){
            dynamicGemLevelAccessList.Remove(i);
        }
        return gems;
    }
}
