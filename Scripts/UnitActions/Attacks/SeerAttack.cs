using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SeerAttack : BasicAttack
{
    [SerializeField]
    private GameObject effectPrefab;

    public override IEnumerator Execute()
    {
        int countL = 0, countR = 0;
        Tuple<int, int> pos = Board.Instance.FindUnitPosition(unit);

        bool leftDone = false, rightDone = false;

        for (int i = 1; i < Board.Instance.GetBoardX(); i++)
        {
            for (int j = Board.Instance.GetBoardZ() - 1; j > 0; j--)
            {
                //Debug.Log("x: " + (pos.Item1 - i) + " z: " + j);
                //Debug.Log(Board.Instance.CheckPositionForUnit(pos.Item1 - i, j, player));

                // Check left side
                if (Board.Instance.CheckPositionForUnit(pos.Item1 - i, j, unit.IsPlayerUnit()) != null)
                {
                    countL++;
                }
                else if (j == Board.Instance.GetBoardZ() - 1) // Break out of both loops if there are no units in this column
                {
                    leftDone = true;
                }

                // Check right side
                if (Board.Instance.CheckPositionForUnit(pos.Item1 + i, j, unit.IsPlayerUnit()) != null)
                {
                    countR++;
                }
                else if (j == Board.Instance.GetBoardZ() - 1) // Break out of both loops if there are no units in this column
                {
                    rightDone = true;
                }


                if (leftDone && rightDone)
                {
                    i = Board.Instance.GetBoardX();
                    break;
                }
            }
        }

        if (countL == countR)
        {
            StartCoroutine(AbilityAnimation());
            yield return new WaitForSeconds(0.5f);

            Stack animations = new Stack();
            List<Unit> units;
            if (unit.IsPlayerUnit())
            {
                units = Board.Instance.GetPlayerUnits();
            }
            else
            {
                units = Board.Instance.GetNPCUnits();
            }

            foreach (Unit ally in units)
            {
                // if(ally != unit)

                Instantiate(effectPrefab, ally.transform);
                
                ally.Heal((int)unit.damage.Value);
                animations.Push(StartCoroutine(FloatingStatChangeSpawner.Instance.SpawnFloatingIcon(ally.transform, ((int)unit.damage.Value).ToString(), Stat.health, true)));
            }

            while (animations.Count > 0)
            {
                var animation = animations.Pop();
                yield return animation;
            }
        }
        else
        {
            yield return StartCoroutine(base.Execute());
        }
    }


    private Tile GetUnitTile(Unit unit)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, 3, transform.position.z), Vector3.down, out hit, Mathf.Infinity, Board.Instance.GetTileLayerMask()) && hit.transform.CompareTag("tile"))
        {
            Tile tile = hit.collider.gameObject.GetComponent<Tile>();
            if (tile != null)
            {
                return tile;
            }
            else
            {
                Debug.LogError("Tile not found");
            }
        }
        else
        {
            Debug.LogError("");
        }
        return null;
    }
}
