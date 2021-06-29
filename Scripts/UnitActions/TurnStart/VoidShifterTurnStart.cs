using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VoidShifterTurnStart : UnitAction
{
    [SerializeField]
    private GameObject effectPrefab;

    public override IEnumerator Execute()
    {
        yield return StartCoroutine(AbilityAnimation());

        // Get position of the unit on the board, the first value is x, second is z
        Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

        if (pos.Item1 == -1)
        {
            Debug.Log("Unit not found on board error");
            yield break;
        }

        // Check the first unit in the opposing col
        Unit target = Board.Instance.CheckPositionForUnit(pos.Item1, 3, !unit.IsPlayerUnit());

        if (target != null)
        {
            if (unit.IsPlayerUnit() && Board.Instance.NPCUnitCount() > 1)
            {
                List<Unit> enemyUnits = Board.Instance.GetNPCUnits();
                enemyUnits.Remove(target);
                Unit swapTarget = enemyUnits[UnityEngine.Random.Range(0, enemyUnits.Count)];

                Tuple<int, int> targetBoardPos = Board.Instance.FindUnitPosition(target);
                Vector3 targetWorldPos = target.transform.position;

                Tuple<int, int> swapTargetBoardPos = Board.Instance.FindUnitPosition(swapTarget);
                Vector3 swapTargetWorldPos = swapTarget.transform.position;

                Board.Instance.RemoveNPCUnit(target, false);
                Board.Instance.RemoveNPCUnit(swapTarget, false);

                Board.Instance.AddNPCUnit(swapTargetBoardPos.Item1, swapTargetBoardPos.Item2, target.gameObject);
                Board.Instance.AddNPCUnit(targetBoardPos.Item1, targetBoardPos.Item2, swapTarget.gameObject);

                target.transform.position = swapTargetWorldPos;
                swapTarget.transform.position = targetWorldPos;
                EventManager.Instance.NPCBoardReorganize();

                Instantiate(effectPrefab, targetWorldPos, Quaternion.identity).transform.LookAt(swapTargetWorldPos);
                Instantiate(effectPrefab, swapTargetWorldPos, Quaternion.identity).transform.LookAt(targetWorldPos);
            }
            else if (!unit.IsPlayerUnit() && Board.Instance.PlayerUnitCount() > 1)
            {
                List<Unit> playerUnits = Board.Instance.GetPlayerUnits();
                playerUnits.Remove(target);
                Unit swapTarget = playerUnits[UnityEngine.Random.Range(0, playerUnits.Count)];

                Tuple<int, int> targetBoardPos = Board.Instance.FindUnitPosition(target);
                Vector3 targetWorldPos = target.transform.position;

                Tuple<int, int> swapTargetBoardPos = Board.Instance.FindUnitPosition(swapTarget);
                Vector3 swapTargetWorldPos = swapTarget.transform.position;

                Board.Instance.RemovePlayerUnit(target, false);
                Board.Instance.RemovePlayerUnit(swapTarget, false);

                Board.Instance.AddPlayerUnit(swapTargetBoardPos.Item1, swapTargetBoardPos.Item2, target.gameObject);
                Board.Instance.AddPlayerUnit(targetBoardPos.Item1, targetBoardPos.Item2, swapTarget.gameObject);

                target.transform.position = swapTargetWorldPos;
                swapTarget.transform.position = targetWorldPos;
                EventManager.Instance.PlayerBoardReorganize();

                Instantiate(effectPrefab, targetWorldPos, Quaternion.identity).transform.LookAt(swapTargetWorldPos);
                Instantiate(effectPrefab, swapTargetWorldPos, Quaternion.identity).transform.LookAt(targetWorldPos);
            }

        }
        else
        {
            //Debug.Log("TARGET NOT FOUND");
        }
    }
}
