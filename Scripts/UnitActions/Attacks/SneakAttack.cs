using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SneakAttack : BasicAttack
{
    [SerializeField]
    private GameObject smokePoof;
    private float multiplier = 1.5f;
    public override IEnumerator Execute()
    {
        // instead of using find target (so the position of the unit does not need to be found twice)
        // Get position of the unit on the board, the first value is x, second is z
        Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

        if (pos.Item1 == -1)
        {
            Debug.Log("Unit not found on board error");
            yield return null;
        }

        // Check the first unit in the opposing col
        Unit target = FindTarget();

        if (target != null)
        {
            HighlightTargetTile(target);
            if (Board.Instance.CheckPositionForUnit(pos.Item1, 2, !unit.IsPlayerUnit()) == null)
            {
                yield return StartCoroutine(MoveAndAttack(target));
            }
            else
            {
                yield return StartCoroutine(AttackAnimation());
                DealDamage(target);
            }
        }
    }

    private IEnumerator MoveAndAttack(Unit target)
    {
        Vector3 oldPos = unit.transform.position;
        Instantiate(smokePoof, oldPos, Quaternion.identity);
        unit.transform.position = new Vector3(target.gameObject.transform.position.x, oldPos.y, target.gameObject.transform.position.z - (target.gameObject.transform.forward.normalized.z));

        if (unit.IsPlayerUnit())
        {
            unit.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            unit.transform.rotation = Quaternion.identity;
        }

        StartCoroutine(AttackAnimation());

        DealDamage(target, multiplier);

        yield return new WaitForSeconds(1f);

        Instantiate(smokePoof, unit.transform.position, Quaternion.identity);
        // Return to original location
        unit.transform.position = oldPos;
        if (unit.IsPlayerUnit())
        {
            unit.transform.rotation = Quaternion.identity;
        }
        else
        {
            unit.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}