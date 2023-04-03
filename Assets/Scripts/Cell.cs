using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Cell : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Cell> nearCells;
    public Building buildingInCell;
    public Button button;
    public Image buttonImg;

    public Depth parentDepth;

    public void Awake(){
        button = GetComponent<Button>();        
        buttonImg = button.GetComponent<Image>();
    }
}
