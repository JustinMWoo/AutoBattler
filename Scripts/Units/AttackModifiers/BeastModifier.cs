using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastModifier : AttackModifier
{
    private float bleedChance;
    private GameObject effectPrefab;

    private int bleedDuration = 3;

    public BeastModifier(Unit unit, float bleedChance, GameObject effectPrefab)
    {
        this.unit = unit;
        this.bleedChance = bleedChance;
        this.effectPrefab = effectPrefab;
    }

    public override void Calculate(Unit target)
    {
        float rand = Random.Range(0f, 1f);

        if (rand <= bleedChance / 100)
        {
            BleedDebuff debuff = new BleedDebuff(target, effectPrefab, bleedDuration);
            TimedDebuffManager.Instance.AddDebuff(target, debuff);
        }
    }
}
