using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinModifier : AttackModifier
{
    protected float CritChance;
    protected float CritMulti;
    private GameObject effectPrefab;
    public AssassinModifier(Unit unit, float CritChance, float CritMulti, GameObject effectPrefab)
    {
        this.unit = unit;
        this.CritChance = CritChance;
        this.CritMulti = CritMulti;
        this.effectPrefab = effectPrefab;
    }

    public override void Calculate(Unit target)
    {
        float rand = Random.Range(0f, 1f);
        //Debug.Log(rand + " <= " + CritChance / 100);
        if (rand <= CritChance / 100)
        {
            Debug.Log("Crit!");
            unit.damage.AddModifier(new StatModifier(CritMulti, StatModType.PercentAdd, this));
            GameObject.Instantiate(effectPrefab, target.transform);
        }
    }
}
