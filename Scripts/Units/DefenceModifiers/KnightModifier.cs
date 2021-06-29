using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightModifier : DefenceModifier
{
    protected List<Unit> redirectTargets;
    protected float percentRedirected;
    protected GameObject redirectEffect;

    public KnightModifier(Unit unit, Unit target, float percentRedirected, GameObject redirectEffect)
    {
        order = 5; // Order set to 5 temporarily

        this.unit = unit;
        this.percentRedirected = percentRedirected;
        this.redirectEffect = redirectEffect;
        redirectTargets = new List<Unit>() { target };
        if (unit.IsPlayerUnit())
            EventManager.Instance.OnPrePlayerBoardReorganize += RemoveModifier;  // THIS NEEDS TO HAPPEN BEFORE KNIGHT UPDATER (this might not be the best place to do this but works for now)
        else
            EventManager.Instance.OnPreNPCBoardReorganize += RemoveModifier;
    }

    public override int Calculate(int damage)
    {
        //Debug.Log("redirecting damage... num:" +redirectTargets.Count); 
        Unit target = redirectTargets[Random.Range(0, redirectTargets.Count)];

        // Create effect
        GameObject.Instantiate(redirectEffect, unit.transform.position, Quaternion.identity).transform.LookAt(target.gameObject.transform);

        target.TakeDamage(DamageCalculator.Instance.CalculateDamage(target, (int)(damage * percentRedirected)));
        return Mathf.CeilToInt(damage * (1 - percentRedirected));
    }
    public void AddAlly(Unit ally)
    {
        redirectTargets.Add(ally);
    }

    public void RemoveModifier()
    {
        unit.RemoveDefenceModifier(this);
    }
}
