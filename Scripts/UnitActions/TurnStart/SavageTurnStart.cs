using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavageTurnStart : UnitAction
{
    private int speedbuff = 2;
    public override IEnumerator Execute()
    {
        unit.speed.AddModifier(new StatModifier(speedbuff, StatModType.Flat, this));
        yield return FloatingStatChangeSpawner.Instance.SpawnFloatingIcon(unit.transform, 2.ToString(), Stat.speed, true);
    }

    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.OnEndCombat += RemoveBuff;
    }

    private IEnumerator RemoveBuff()
    {
        unit.speed.RemoveAllModifiersFromSource(this);
        yield return null;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnEndCombat -= RemoveBuff;
    }
}
