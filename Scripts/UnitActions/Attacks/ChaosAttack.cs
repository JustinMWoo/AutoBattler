using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChaosAttack : BasicAttack
{
    [SerializeField]
    private GameObject effectPrefab;
    private BasicAttack oldAttack;

    public override IEnumerator Execute()
    {
        // Deal damage to self
        int damage = (int) unit.maxHealth.Value / 5;
        if (unit.GetCurrentHealth() - damage <= 0)
        {
            damage = unit.GetCurrentHealth() - 1;
        }
        DamageCalculator.Instance.DamageText(unit, damage);
        unit.TakeDamage(damage);

        List<Unit> allTargets = new List<Unit>();
        yield return StartCoroutine(AttackAnimation());

        // Get position of the unit on the board, the first value is x, second is z
        Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

        if (pos.Item1 == -1)
        {
            Debug.Log("Unit not found on board error");
            yield break;
        }

        // Check for second unit in the opposing col
        Unit target = Board.Instance.CheckPositionForUnit(pos.Item1, 2, !unit.IsPlayerUnit());

        // Get y position for tiles
        RaycastHit hit;
        float ypos = 0;
        if (Physics.Raycast(new Vector3(pos.Item1, 3, pos.Item2), Vector3.down, out hit, Mathf.Infinity, layerMask) && hit.transform.CompareTag("tile"))
        {
            ypos = hit.point.y;
        }


        if (target != null)
        {
            allTargets.Add(target);
        }

        if (unit.IsPlayerUnit())
        {
            Instantiate(effectPrefab, new Vector3(pos.Item1, ypos, Board.Instance.GetNPCZOffset() + 1), Quaternion.identity);
        }
        else
        {
            Instantiate(effectPrefab, new Vector3(pos.Item1, ypos, 2), Quaternion.identity);
        }

        // time before spawning outer part of pattern
        yield return new WaitForSeconds(0.5f);

        if (pos.Item1 - 1 >= 0)
        {
            // Check back left
            target = Board.Instance.CheckPositionForUnit(pos.Item1 - 1, 1, !unit.IsPlayerUnit());

            if (target != null)
            {
                allTargets.Add(target);
            }

            // Check front left
            target = Board.Instance.CheckPositionForUnit(pos.Item1 - 1, 3, !unit.IsPlayerUnit());

            if (target != null)
            {
                allTargets.Add(target);
            }

            if (unit.IsPlayerUnit())
            {
                Instantiate(effectPrefab, new Vector3(pos.Item1 - 1, ypos, Board.Instance.GetNPCZOffset() + 2), Quaternion.identity);
                Instantiate(effectPrefab, new Vector3(pos.Item1 - 1, ypos, Board.Instance.GetNPCZOffset()), Quaternion.identity);
            }
            else
            {
                Instantiate(effectPrefab, new Vector3(pos.Item1 - 1, ypos, 3), Quaternion.identity);
                Instantiate(effectPrefab, new Vector3(pos.Item1 - 1, ypos, 1), Quaternion.identity);
            }
        }


        if (pos.Item1 + 1 < Board.Instance.GetBoardX())
        {
            // Check back right
            target = Board.Instance.CheckPositionForUnit(pos.Item1 + 1, 1, !unit.IsPlayerUnit());

            if (target != null)
            {
                allTargets.Add(target);
            }

            // Check front right
            target = Board.Instance.CheckPositionForUnit(pos.Item1 + 1, 3, !unit.IsPlayerUnit());

            if (target != null)
            {
                allTargets.Add(target);
            }

            if (unit.IsPlayerUnit())
            {
                Instantiate(effectPrefab, new Vector3(pos.Item1 + 1, ypos, Board.Instance.GetNPCZOffset() + 2), Quaternion.identity);
                Instantiate(effectPrefab, new Vector3(pos.Item1 + 1, ypos, Board.Instance.GetNPCZOffset()), Quaternion.identity);
            }
            else
            {
                Instantiate(effectPrefab, new Vector3(pos.Item1 + 1, ypos, 3), Quaternion.identity);
                Instantiate(effectPrefab, new Vector3(pos.Item1 + 1, ypos, 1), Quaternion.identity);
            }
        }


        foreach (Unit enemy in allTargets)
        {
            DealDamage(enemy);
            HighlightTargetTile(enemy);
        }

        yield return new WaitForSeconds(0.5f);
    }

    protected override void Awake()
    {
        base.Awake();
        oldAttack = unit.AttackType;
        unit.AttackType = this;

        EventManager.Instance.OnEndCombat += ResetAttack;
    }
    private IEnumerator ResetAttack()
    {
        EventManager.Instance.OnEndCombat -= ResetAttack;
        unit.RemoveTurnStart(this);
        unit.AttackType = oldAttack;

        Destroy(this.gameObject);
        yield return null;
    }

    // If enemy unit is killed to prevent error
    private void OnDestroy()
    {
        EventManager.Instance.OnEndCombat -= ResetAttack;
    }
}
