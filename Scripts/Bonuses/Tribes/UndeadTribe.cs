using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tribes/Undead")]
public class UndeadTribe : BaseTribe
{
    private int playerValue;
    private int npcValue;

    private void Awake()
    {
        tierRequirements = new List<int> { 2, 4 };
    }

    public override IEnumerator ApplyBonus(int count, bool player)
    {
        Stack animations = new Stack();

        if (count < tierRequirements[0])
        {
            yield break;
        }
        else if (count < tierRequirements[1])
        {
            if (player)
            {
                EventManager.Instance.OnCycleStart += PlayerUndeadBonus;
                playerValue = -1;
            }
            else
            {
                EventManager.Instance.OnCycleStart += NPCUndeadBonus;
                npcValue = -1;
            }
        }
        else
        {
            if (player)
            {
                EventManager.Instance.OnCycleStart += PlayerUndeadBonus;
                playerValue = -2;
            }
            else
            {
                EventManager.Instance.OnCycleStart += NPCUndeadBonus;
                npcValue = -2;
            }
        }
    }

    public IEnumerator PlayerUndeadBonus()
    {
        List<Unit> units;

        Stack animations = new Stack();
        // Get the other team's units

        units = Board.Instance.GetNPCUnits();

        foreach (Unit unit in units)
        {
            unit.defence.AddModifier(new StatModifier(playerValue, StatModType.Flat, this));
            //animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
            animations.Push(BonusManager.Instance.StartCoroutine(FloatingStatChangeSpawner.Instance.SpawnFloatingIcon(unit.transform, playerValue.ToString(), Stat.defence, false)));
        }
        while (animations.Count > 0)
        {
            var animation = animations.Pop();
            yield return animation;
        }
    }

    public IEnumerator NPCUndeadBonus()
    {
        List<Unit> units;

        Stack animations = new Stack();

        // Get the other team's units
        units = Board.Instance.GetPlayerUnits();

        foreach (Unit unit in units)
        {
            unit.defence.AddModifier(new StatModifier(npcValue, StatModType.Flat, this));
            //animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
            animations.Push(BonusManager.Instance.StartCoroutine(FloatingStatChangeSpawner.Instance.SpawnFloatingIcon(unit.transform, npcValue.ToString(), Stat.defence, false)));
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

        if (!player)
        {
            units = Board.Instance.GetPlayerUnits();
        }
        else
        {
            units = Board.Instance.GetNPCUnits();
        }

        foreach (Unit unit in units)
        {
            unit.defence.RemoveAllModifiersFromSource(this);
        }

        EventManager.Instance.OnCycleStart -= PlayerUndeadBonus;
        EventManager.Instance.OnCycleStart -= NPCUndeadBonus;
    }
}

