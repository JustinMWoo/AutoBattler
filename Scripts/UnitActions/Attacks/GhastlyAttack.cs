using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhastlyAttack : BasicAttack
{
    private readonly float executeThreshold = 0.2f;
    private readonly float cleaveValue = 0.5f;
    public GameObject executeEffectPrefab;

    public override IEnumerator Execute()
    {
        Unit target = FindTarget();

        if (target != null && target.GetCurrentHealth() <= target.maxHealth.Value * executeThreshold)
        {
            yield return StartCoroutine(AbilityAnimation());

            Instantiate(executeEffectPrefab, target.transform);

            yield return StartCoroutine(target.Die());
        }
        else
        {
            List<Unit> cleaveTargets = new List<Unit>();
            yield return StartCoroutine(AttackAnimation());
            Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

            if (target != null)
            {
                cleaveTargets.Add(target);
            }

            Unit cleaveTarget = Board.Instance.CheckPositionForUnit(pos.Item1 - 1, 3, !unit.IsPlayerUnit()); // Check left
            if (cleaveTarget != null)
            {
                cleaveTargets.Add(cleaveTarget);
            }

            cleaveTarget = Board.Instance.CheckPositionForUnit(pos.Item1 + 1, 3, !unit.IsPlayerUnit()); // Check right
            if (cleaveTarget != null)
            {
                cleaveTargets.Add(cleaveTarget);
            }

            foreach (Unit targets in cleaveTargets)
            {
                DealDamage(targets, cleaveValue);
            }
        }
    }
}
