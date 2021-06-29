using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tribes/Beast")]
public class BeastTribe : BaseTribe
{
    [SerializeField]
    private GameObject effectPrefab;

    private void Awake()
    {
        tierRequirements = new List<int> { 2, 4 };
    }

    public override IEnumerator ApplyBonus(int tribeCount, bool player)
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

        if (tribeCount < tierRequirements[0])
        {
            yield break;
        }
        else if (tribeCount < tierRequirements[1])
        {
            foreach (Unit unit in units)
            {
                AttackModifier mod = new BeastModifier(unit, 15, effectPrefab);
                unit.AddAttackModifier(mod);
                animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
            }
        }
        else
        {
            foreach (Unit unit in units)
            {
                AttackModifier mod = new BeastModifier(unit, 30, effectPrefab);
                unit.AddAttackModifier(mod);
                animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
            }
        }

        while (animations.Count > 0)
        {
            var animation = animations.Pop();
            yield return animation;
        }
    }
}
