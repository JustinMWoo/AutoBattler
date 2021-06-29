using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChainAttack : BasicAttack
{
    public override IEnumerator Execute()
    {
        List<Unit> chainTargets = new List<Unit>();
        yield return StartCoroutine(AttackAnimation());
        Unit target = FindTarget();

        if (target != null)
        {
            Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);
            Unit chainTarget = Board.Instance.CheckPositionForUnit(pos.Item1, 2, !unit.IsPlayerUnit()); // Check behind
            if (chainTarget != null)
            {
                // Debug.Log("Targed found behind");
                chainTargets.Add(chainTarget);
            }

            chainTarget = Board.Instance.CheckPositionForUnit(pos.Item1 + 1, 3, !unit.IsPlayerUnit()); // Check right
            if (chainTarget != null)
            {
                // Debug.Log("Targed found right");
                chainTargets.Add(chainTarget);
            }

            chainTarget = Board.Instance.CheckPositionForUnit(pos.Item1 - 1, 3, !unit.IsPlayerUnit()); // Check left
            if (chainTarget != null)
            {
                // Debug.Log("Targed found left");
                chainTargets.Add(chainTarget);
            }

            HighlightTargetTile(target);
            DealDamage(target);

            if (chainTargets.Count > 0)
            {
                chainTarget = chainTargets[UnityEngine.Random.Range(0, chainTargets.Count - 1)];
                // Deal half damage to the selected chain target
                DealDamage(chainTarget, 0.5f);
                HighlightTargetTile(chainTarget);
            }
        }
    }
}