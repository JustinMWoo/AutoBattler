using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Sets/Balance")]
public class BalanceSet : BaseSet
{
    private void Awake()
    {
        tierRequirements = new List<int> { 2, 4 };
    }
    public override IEnumerator ApplyBonus(int setCount, bool player)
    {
        List<Unit> units;

        if (player)
        {
            units = Board.Instance.GetPlayerUnits();
        }
        else
        {
            units = Board.Instance.GetNPCUnits();
        }

        Stack animations = new Stack();

        if (setCount < tierRequirements[0])
        {
            yield break;
        }
        else if (setCount < tierRequirements[1])
        {
            foreach (Unit unit in units)
            {
                if (unit.Set == this)
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
                            if (Board.Instance.CheckPositionForUnit(pos.Item1 - i, j, player) != null)
                            {
                                countL++;
                            }
                            else if (j == Board.Instance.GetBoardZ() - 1) // Break out of both loops if there are no units in this column
                            {
                                leftDone = true;
                            }

                            // Check right side
                            if (Board.Instance.CheckPositionForUnit(pos.Item1 + i, j, player) != null)
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
                    int damageTotal = countL * 2;
                    int defenceTotal = (int)(countR * 0.5f);

                    //animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));

                    unit.damage.AddModifier(new StatModifier(damageTotal, StatModType.Flat));
                    unit.defence.AddModifier(new StatModifier(defenceTotal, StatModType.Flat));
                    animations.Push(FloatingStatChangeSpawner.Instance.StartCoroutine(SpawnBuffPrefabs(unit.transform, new List<string>() {null, damageTotal.ToString(), defenceTotal.ToString() })));
                }
            }
        }
        else
        {
            foreach (Unit unit in units)
            {
                if (unit.Set == this)
                {

                    animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
                }
            }
        }

        while (animations.Count > 0)
        {
            var animation = animations.Pop();
            yield return animation;
        }
    }
}
