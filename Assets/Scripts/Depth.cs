using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depth : MonoBehaviour
{
    public int depthNumber;
    public int goldIncome;
    public bool depthAvailable;
    public List<DepthRow> rowList;
    public List<Cell> buildingList;

    public void Awake(){
        foreach(DepthRow depthRow in rowList){
            foreach(Cell c in depthRow.cellList){
                c.parentDepth = this;
            }
        }
    }

    public int getDigBalance(){
        int i = 0;
        foreach(DepthRow row in rowList){
            if(row.currentCountOfDigToAccess > 0){
                i += row.currentCountOfDigToAccess;
            }
        }
        return i;
    }

    public bool depthContainBuildings(string buildingName){
        foreach(Cell c in buildingList){
            if(c.buildingInCell.props.name == buildingName){
                return true;
            }
        }
        return false;
    }

    public int getDepthsFreeCellsCount(){
        int counter = 0;
        foreach(DepthRow row in rowList){
            if(!row.accessToRowAllowed){
                break;
            }
            foreach(Cell c in row.cellList){
                if(c.buildingInCell == null ){
                    counter++;
                }
            }
        }
        return counter;
    }
}
