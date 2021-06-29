using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RangerModifier : AttackModifier
{
    protected int damagePerTile;
    public RangerModifier(Unit unit, int damage)
    {
        this.unit = unit;
        damagePerTile = damage;
    }

    public override void Calculate(Unit target)
    {
        // Get position of the unit on the board, the first value is x, second is z
        Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

        if (pos.Item1 == -1)
        {
            Debug.Log("Unit not found on board error");
            return;
        }

        if (target == null)
        {
            return;
        }
        Tuple<int, int> targetPos = Board.Instance.FindUnitPosition(target);

        int numTiles = (3 - targetPos.Item2) + (3 - pos.Item2); // Find the tile distance between the targets (in z value only)
        //Debug.Log("RangerBonus = " + numTiles * DamagePerTile);

        unit.damage.AddModifier(new StatModifier(numTiles * damagePerTile, StatModType.Flat, this));
    }
}
