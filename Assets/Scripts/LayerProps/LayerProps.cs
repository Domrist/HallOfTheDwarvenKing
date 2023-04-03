using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName="Layer",menuName="HallOfTheDwarvenKing/Create new depth",order=51)]
public class LayerProps : ScriptableObject{
    public byte depth;
    public int gold;
    public byte digPerRow;
    public List<int> diamondsDigDepth;
    public Button b;
}
