using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tribes/Human")]
public class HumanTribe : BaseTribe
{
    private void Awake()
    {
        tierRequirements = new List<int> { 2, 4 };
    }
    public override IEnumerator ApplyBonus(int count, bool player)
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

        if (count < tierRequirements[0])
        {
            yield break;
        }
        else if (count < tierRequirements[1])
        {
            foreach (Unit unit in units)
            {
                unit.maxHealth.AddModifier(new StatModifier(5, StatModType.Flat, this));
                unit.damage.AddModifier(new StatModifier(2, StatModType.Flat, this));
                unit.speed.AddModifier(new StatModifier(3, StatModType.Flat, this));
                unit.defence.AddModifier(new StatModifier(1, StatModType.Flat, this));
                animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefabs(unit.transform, new List<string>() {null, 5.ToString(), 2.ToString(), 3.ToString(), 1.ToString() })));
            }

        }
        else
        {
            foreach (Unit unit in units)
            {
                unit.maxHealth.AddModifier(new StatModifier(10, StatModType.Flat, this));
                unit.damage.AddModifier(new StatModifier(4, StatModType.Flat, this));
                unit.speed.AddModifier(new StatModifier(6, StatModType.Flat, this));
                unit.defence.AddModifier(new StatModifier(2, StatModType.Flat, this));
                animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefabs(unit.transform, new List<string>() {null, 10.ToString(), 4.ToString(), 6.ToString(), 2.ToString() })));
            }
        }

        while (animations.Count > 0)
        {
            var animation = animations.Pop();
            yield return animation;
        }
    }
    public override void RemoveBonus(bool player)
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

        foreach (Unit unit in units)
        {
            unit.maxHealth.RemoveAllModifiersFromSource(this);
            unit.damage.RemoveAllModifiersFromSource(this);
            unit.speed.RemoveAllModifiersFromSource(this);
            unit.defence.RemoveAllModifiersFromSource(this);
        }
    }
}
