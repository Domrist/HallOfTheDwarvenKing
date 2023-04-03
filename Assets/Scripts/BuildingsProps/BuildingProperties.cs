using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName="Building",menuName="HallOfTheDwarvenKing/Create new building",order=51)]
public class BuildingProperties : ScriptableObject
{
    public string name;

    public Sprite icon;

    public BuildingType type;

    public enum BuildingType{
        Mine,
        Still,
        Tavern,
        Statue,
        Arena,
        GamingDen,
        Blacksmith,
        Carpenter,
        MushroomFarm,
        Vault,
        ConcertHall,
        ThroneRoom
    }

    public int cost;
    public char spaceRequired;

    public bool unique;
    public bool alreadyBuilded;

    public int id;
}