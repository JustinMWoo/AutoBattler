using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostAttack : BasicAttack
{
    private int speedDebuff = -3;
    public override IEnumerator Execute()
    {
        yield return StartCoroutine(AttackAnimation());

        Unit target = FindTarget();

        if (target != null)
        {
            HighlightTargetTile(target);
            // Deal damage to unit
            DealDamage(target);
            target.speed.AddModifier(new StatModifier(speedDebuff, StatModType.Flat));
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(FloatingStatChangeSpawner.Instance.SpawnFloatingIcon(target.transform, speedDebuff.ToString(), Stat.speed, false));
        }
        else
        {
            //Debug.Log("TARGET NOT FOUND");
        }

    }
}
