using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sets/Ranger")]
public class RangerSet : BaseSet
{
    //[SerializeField]
    //private GameObject RangerTurnStart;
    //[SerializeField]
    //private GameObject RangerTurnEnd;
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
                    AttackModifier mod = new RangerModifier(unit, 2);
                    unit.AddAttackModifier(mod);
                    animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
                }
            }
        }
        else
        {
            foreach (Unit unit in units)
            {
                if (unit.Set == this)
                {
                    AttackModifier mod = new RangerModifier(unit, 4);
                    unit.AddAttackModifier(mod);
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
