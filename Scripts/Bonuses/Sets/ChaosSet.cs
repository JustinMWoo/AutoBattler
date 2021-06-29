using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sets/Chaos")]
public class ChaosSet : BaseSet
{
    [SerializeField]
    private GameObject chaosAttackPrefab;

    private void Awake()
    {
        tierRequirements = new List<int> { 2, 4 };
    }

    public override IEnumerator ApplyBonus(int setCount, bool player)
    {
        List<Unit> allUnits;
        List<Unit> units = new List<Unit>();

        if (player)
        {
            allUnits = Board.Instance.GetPlayerUnits();
        }
        else
        {
            allUnits = Board.Instance.GetNPCUnits();
        }

        Stack animations = new Stack();

        // Get all units from this set
        foreach (Unit unit in allUnits)
        {
            if (unit.Set == this)
            {
                units.Add(unit);
            }
        }

        Unit target;

        if (setCount < tierRequirements[0])
        {
            yield break;
        }
        else if (setCount < tierRequirements[1])
        {
            target = units[Random.Range(0, units.Count)];
            animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(target.transform)));
            Instantiate(chaosAttackPrefab, target.transform);
        }
        else
        {
            target = units[Random.Range(0, units.Count)];
            animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(target.transform)));
            Instantiate(chaosAttackPrefab, target.transform);
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
        }
    }
}
