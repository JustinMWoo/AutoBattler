using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KnightUpdater : MonoBehaviour
{
    public float percentRedirected;
    [SerializeField]
    protected GameObject redirectEffectPrefab;
    private Unit unit;
    private void Start()
    {
        unit = GetComponentInParent<Unit>();
        if (unit.IsPlayerUnit())
        {
            EventManager.Instance.OnPlayerBoardReorganize += UpdateBonuses;
        }
        else
        {
            EventManager.Instance.OnNPCBoardReorganize += UpdateBonuses;
        }
        UpdateBonuses();
    }
    public void UpdateBonuses()
    {
        Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

        // Check in front
        Unit ally = Board.Instance.CheckPositionForUnit(pos.Item1, pos.Item2 + 1, unit.IsPlayerUnit());
        CheckAddModifier(ally);

        // Check behind
        ally = Board.Instance.CheckPositionForUnit(pos.Item1, pos.Item2 - 1, unit.IsPlayerUnit());
        CheckAddModifier(ally);

        // Check left
        ally = Board.Instance.CheckPositionForUnit(pos.Item1 - 1, pos.Item2, unit.IsPlayerUnit());
        CheckAddModifier(ally);

        // Check right
        ally = Board.Instance.CheckPositionForUnit(pos.Item1 + 1, pos.Item2, unit.IsPlayerUnit());
        CheckAddModifier(ally);
    }
    private void CheckAddModifier(Unit ally)
    {
        if (ally != null)
        {
            if (ally.Set is KnightSet)
            {
                return;
            }

            foreach (DefenceModifier mod in ally.GetDefenceModifiers())
            {
                if (mod is KnightModifier modifier)
                {
                    modifier.AddAlly(unit);
                    return;
                }
            }
            //Debug.Log("modifier attached... player unit = " + ally.IsPlayerUnit());
            ally.AddDefenceModifier(new KnightModifier(ally, unit, percentRedirected, redirectEffectPrefab));
        }
    }

    private void Awake()
    {
        EventManager.Instance.OnEndCombat += RemoveBonus;
    }
    private IEnumerator RemoveBonus()
    {
        EventManager.Instance.OnEndCombat -= RemoveBonus;
        Destroy(this.gameObject);
        yield return null;
    }
}
