using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BasicAttack : UnitAction
{
    public override IEnumerator Execute()
    {
        yield return StartCoroutine(AttackAnimation());

        Unit target = FindTarget();

        if (target != null)
        {
            HighlightTargetTile(target);
            // Deal damage to unit
            DealDamage(target);
        }
        else
        {
            //Debug.Log("TARGET NOT FOUND");
        }

    }

    protected void DealDamage(Unit target)
    {
        target.TakeDamage(DamageCalculator.Instance.CalculateDamage(unit, target));
    }

    protected void DealDamage(Unit target, float multiplier)
    {
        target.TakeDamage(DamageCalculator.Instance.CalculateDamage(unit, target, multiplier));
    }

    /*
     * Finds the primary target for the unit's attack, override if target selection needs to be changed!
     */
    public virtual Unit FindTarget()
    {
        // Get position of the unit on the board, the first value is x, second is z
        Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

        if (pos.Item1 == -1)
        {
            Debug.Log("Unit not found on board error");
            return null;
        }

        // Check the first unit in the opposing col
        return Board.Instance.CheckPositionForUnit(pos.Item1, 3, !unit.IsPlayerUnit());
    }
    
}