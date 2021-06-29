using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tribes/Orc")]
public class OrcTribe : BaseTribe
{
    [SerializeField]
    private GameObject OrcPrefab;

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
                GameObject action = Instantiate(OrcPrefab, unit.transform);
                action.GetComponent<OrcTurnEnd>().Heal = 3;
                unit.AddTurnEnd(action.GetComponent<OrcTurnEnd>());
                unit.maxHealth.AddModifier(new StatModifier(10, StatModType.Flat, this));
                animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefabs(unit.transform, new List<string>() { null, 10.ToString() })));
            }

        }
        else
        {
            foreach (Unit unit in units)
            {
                GameObject action = Instantiate(OrcPrefab, unit.transform);
                action.GetComponent<OrcTurnEnd>().Heal = 5;
                unit.AddTurnEnd(action.GetComponent<OrcTurnEnd>());
                unit.maxHealth.AddModifier(new StatModifier(20, StatModType.Flat, this));
                animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefabs(unit.transform, new List<string>() { null, 20.ToString() })));
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
