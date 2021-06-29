using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnOfChaosAttack : BasicAttack
{
    [SerializeField]
    private GameObject effectPrefab;

    private Tuple<int, int> pos;


    public override IEnumerator Execute()
    {
        pos = Board.Instance.FindUnitPosition(unit);
        if (pos.Item1 == -1)
        {
            Debug.Log("Unit not found on board error");
            yield break;
        }

        int size;

        if (unit.Level == 1)
        {
            yield return base.Execute();
            yield break;
        }
        else if (unit.Level == 2)
        {
            size = 1;
        }
        else
        {
            size = 2;
        }
        yield return StartCoroutine(AbilityAnimation());

        // Get y position for tiles
        RaycastHit hit;
        float ypos = 0;
        if (Physics.Raycast(new Vector3(pos.Item1, 3, pos.Item2), Vector3.down, out hit, Mathf.Infinity, layerMask) && hit.transform.CompareTag("tile"))
        {
            ypos = hit.point.y;
        }

        // Find first position in opposing column
        List<Unit> targets = new List<Unit>();
        Unit target = Board.Instance.CheckPositionForUnit(pos.Item1, 3, !unit.IsPlayerUnit());
        if (target != null)
        {
            targets.Add(target);
        }

        SpawnEffect(pos.Item1, ypos, Board.Instance.GetBoardZ(), size);


        for (int i = 1; i <= size; i++)
        {
            if (Board.Instance.GetBoardZ() - i >= 0)
            {
                // Behind
                target = Board.Instance.CheckPositionForUnit(pos.Item1, Board.Instance.GetBoardZ() - 1 - i, !unit.IsPlayerUnit());
                if (target != null)
                {
                    Debug.Log("Behind: " + target.UnitData.unitName);
                    targets.Add(target);
                }
            }
            if (pos.Item1 - i >= 0)
            {
                // Left
                target = Board.Instance.CheckPositionForUnit(pos.Item1 - i, Board.Instance.GetBoardZ() - 1, !unit.IsPlayerUnit());
                if (target != null)
                {
                    Debug.Log("Left: " + target.UnitData.unitName);
                    targets.Add(target);
                }
            }

            if (pos.Item1 + i < Board.Instance.GetBoardX())
            {
                // Right
                target = Board.Instance.CheckPositionForUnit(pos.Item1 + i, Board.Instance.GetBoardZ() - 1, !unit.IsPlayerUnit());
                if (target != null)
                {
                    Debug.Log("Right: " + target.UnitData.unitName);
                    targets.Add(target);
                }
            }
        }

        foreach (Unit enemy in targets)
        {
            //Debug.Log(enemy.UnitData.unitName);
            DealDamage(enemy);
            HighlightTargetTile(enemy);
        }
    }

    private void SpawnEffect(int boardX, float y, int boardZ, int size)
    {
        GameObject effect;
        if (unit.IsPlayerUnit())
        {
            effect = Instantiate(effectPrefab, new Vector3(boardX, y, Board.Instance.GetNPCZOffset() + (Board.Instance.GetBoardZ() - boardZ)), Quaternion.identity);
        }
        else
        {
            effect = Instantiate(effectPrefab, new Vector3(boardX, y, boardZ), Quaternion.identity);
        }

        ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in particleSystems)
        {
            var velocity = ps.velocityOverLifetime;
            velocity.speedModifierMultiplier = size;
        }
    }
}
