using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidModifier : AttackModifier
{
    protected float stunChance;
    protected GameObject stunPrefab;

    public VoidModifier(Unit unit, float stunChance, GameObject stunPrefab)
    {
        this.stunChance = stunChance;
        this.unit = unit;
        this.stunPrefab = stunPrefab;
    }

    public override void Calculate(Unit target)
    {
        float rand = Random.Range(0f, 1f);
        //Debug.Log(rand + " <= " + CritChance / 100);
        if (rand <= stunChance / 100)
        {
            Debug.Log("stun!");
            GameObject stun =  GameObject.Instantiate(stunPrefab, target.transform);
            stun.GetComponent<Stun>().Initialize();
        }
    }
}
