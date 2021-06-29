using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwiftShotAttack : BasicAttack
{
    private int cooldown = 3;
    private int count = 0;

    public override IEnumerator Execute()
    {
        count++;
        if (count < cooldown)
        {
            yield return StartCoroutine(base.Execute());
        }
        else
        {
            // ability attack
            yield return StartCoroutine(AbilityAnimation());

            Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

            if (pos.Item1 == -1)
            {
                Debug.LogError("Unit not found on board error");
                yield return null;
            }

            int i = 3;
            Unit target = Board.Instance.CheckPositionForUnit(pos.Item1, i, !unit.IsPlayerUnit());
            while (target != null)
            {
                DealDamage(target);
                HighlightTargetTile(target);
                target = Board.Instance.CheckPositionForUnit(pos.Item1, --i, !unit.IsPlayerUnit());
            }
            count = 0;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.OnCombatStart += ResetCount;
    }
    private IEnumerator ResetCount()
    {
        count = 0;
        yield return null;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnCombatStart -= ResetCount;
    }
}
