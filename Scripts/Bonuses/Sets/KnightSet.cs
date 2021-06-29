using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sets/Knight")]
public class KnightSet : BaseSet
{
    [SerializeField]
    private GameObject knightPrefab;
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
                    animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
                    Instantiate(knightPrefab, unit.transform).GetComponent<KnightUpdater>().percentRedirected = 0.5f;
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
                    Instantiate(knightPrefab, unit.transform).GetComponent<KnightUpdater>().percentRedirected = 0.75f;
                }
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
        }
    }
}
