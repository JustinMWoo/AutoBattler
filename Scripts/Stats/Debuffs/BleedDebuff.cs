using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedDebuff : Debuff
{
    private int damage = 5;
    private GameObject effectPrefab;
    public BleedDebuff(Unit unit, GameObject effectPrefab, int max) : base(unit, max)
    {
        this.effectPrefab = effectPrefab;
    }

    public override IEnumerator Execute()
    {
        TimedDebuffManager.Instance.StartCoroutine(base.Execute());

        DamageCalculator.Instance.DamageText(unit, damage);
        unit.TakeDamage(damage);

        GameObject.Instantiate(effectPrefab, unit.transform);
        yield return new WaitForSeconds(0.5f);
    }
}
