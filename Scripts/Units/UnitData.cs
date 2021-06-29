using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Contains the stats and model of the unit to load from 
 */
[CreateAssetMenu(menuName = "UnitData")]
public class UnitData : ScriptableObject
{
    public string unitName;
    [TextArea(15,20)]
    public string description;

    [Header("Event actions")]
    public GameObject attackType;
    public GameObject turnStart;
    public GameObject turnEnd;
    public GameObject onHealthChanged;
    public GameObject onDeath;

    [Header("Stats")]
    public int speed;
    public int damage;
    public int health;
    public int defence;
    public BaseSet set;
    public BaseTribe tribe;

    [Header("Model")]
    public GameObject modelPrefab;
}
