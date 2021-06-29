using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RangerAttack : BasicAttack
{
    /*
     * Finds the target that is the furthest back in the column
     */
    public override Unit FindTarget()
    {
        Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

        if (pos.Item1 == -1)
        {
            Debug.Log("Unit not found on board error");
            return null;
        }
        int i = 3;
        Unit target = Board.Instance.CheckPositionForUnit(pos.Item1, i, !unit.IsPlayerUnit());
        Unit preTarget = null;
        while (target != null)
        {
            preTarget = target;
            target = Board.Instance.CheckPositionForUnit(pos.Item1, --i, !unit.IsPlayerUnit());
        }
        return preTarget;
    }
}
