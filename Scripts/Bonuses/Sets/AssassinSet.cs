using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sets/Assassin")]
public class AssassinSet : BaseSet
{
    //[SerializeField]
    //private GameObject AssassinTurnStartPrefab;
    [SerializeField]
    private GameObject effectPrefab;

    private void Awake()
    {
        tierRequirements = new List<int> { 2, 6 };
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
                    AttackModifier mod = new AssassinModifier(unit, 40, 0.5f, effectPrefab);
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
                    AttackModifier mod = new AssassinModifier(unit, 80, 1f, effectPrefab);
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
