using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class MageTurnStart : UnitAction
{
    public int manaThreshold;
    protected float currentMana = 0;
    [SerializeField]
    protected GameObject textPrefab;
    [SerializeField]
    protected GameObject absorbEffect;
    [SerializeField]
    protected GameObject mageAttack;

    public override IEnumerator Execute()
    {
        // Get position of the unit on the board, the first value is x, second is z
        Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

        if (pos.Item1 == -1)
        {
            Debug.Log("Unit not found on board error");
            yield break;
        }

        List<Unit> enemies = new List<Unit>();

        for (int i = Board.Instance.GetBoardZ() - 1; i >= 0; i--)
        {
            Unit enemy = Board.Instance.CheckPositionForUnit(pos.Item1, i, !unit.IsPlayerUnit());
            if (enemy != null)
            {
                enemies.Add(enemy);
            }
            else
            {
                break;
            }
        }

        foreach (Unit enemy in enemies)
        {
            currentMana += enemy.maxHealth.Value * 0.5f;
            GameObject effect = GameObject.Instantiate(absorbEffect, enemy.transform);
            effect.GetComponentInChildren<ParticleSystemForceField>().gameObject.transform.position = unit.transform.position;
        }

        GameObject.Instantiate(textPrefab, unit.transform).GetComponent<FloatingIcon>().SetText(currentMana.ToString() + "/" + manaThreshold);

        if (currentMana >= manaThreshold)
        {
            GameObject.Instantiate(mageAttack, unit.transform);
            currentMana = 0;
        }
        yield return new WaitForSeconds(1);
    }

    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.OnEndCombat += RemoveBonus;
    }
    private IEnumerator RemoveBonus()
    {
        EventManager.Instance.OnEndCombat -= RemoveBonus;
        unit.RemoveTurnStart(this);
        Destroy(this.gameObject);
        yield return null;
    }

    // If enemy unit is killed to prevent error
    private void OnDestroy()
    {
        EventManager.Instance.OnEndCombat -= RemoveBonus;
    }
}
