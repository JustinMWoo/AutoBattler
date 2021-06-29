using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MageAttack : BasicAttack
{
    public GameObject AttackEffect;
    public override IEnumerator Execute()
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
        if (unit.IsPlayerUnit())
        {
            Instantiate(AttackEffect, unit.transform);
        }
        else
        {
            Instantiate(AttackEffect, unit.transform.position, Quaternion.Euler(0, 180, 0));
        }

        while (target != null)
        {
            DealDamage(target, 1.5f);
            HighlightTargetTile(target);
            target = Board.Instance.CheckPositionForUnit(pos.Item1, --i, !unit.IsPlayerUnit());
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (unit.IsPlayerUnit())
        {
            EventManager.Instance.OnPlayerAttack -= unit.AttackType.Execute;
            EventManager.Instance.OnPlayerAttack += Execute;
        }
        else
        {
            EventManager.Instance.OnNPCAttack -= unit.AttackType.Execute;
            EventManager.Instance.OnNPCAttack += Execute;
        }

        EventManager.Instance.OnEndTurn += RemoveAttack;
    }

    private IEnumerator RemoveAttack()
    {
        if (unit.IsPlayerUnit())
        {
            EventManager.Instance.OnPlayerAttack -= Execute;
        }
        else
        {
            EventManager.Instance.OnNPCAttack -= Execute;
        }

        EventManager.Instance.OnEndTurn -= RemoveAttack;
        Destroy(this.gameObject);
        yield return null;
    }
}
