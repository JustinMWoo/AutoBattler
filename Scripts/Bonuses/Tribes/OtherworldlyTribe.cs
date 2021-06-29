using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tribes/Otherwordly")]
public class OtherworldlyTribe : BaseTribe
{
    [SerializeField]
    private GameObject effectPrefab;

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
                if (unit.Tribe == this)
                {
                    animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
                }
            }

            if (player)
            {
                OtherworldlyBonus bonus = new OtherworldlyBonus(30f, effectPrefab);
                EventManager.Instance.OnNPCUnitDeath += bonus.ApplyBonus;
            }
            else
            {
                OtherworldlyBonus bonus = new OtherworldlyBonus(30f, effectPrefab);
                EventManager.Instance.OnPlayerUnitDeath += bonus.ApplyBonus;
            }
        }
        else
        {
            foreach (Unit unit in units)
            {
                if (unit.Tribe == this)
                {
                    animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
                }
            }

            if (player)
            {
                OtherworldlyBonus bonus = new OtherworldlyBonus(50f, effectPrefab);
                EventManager.Instance.OnNPCUnitDeath += bonus.ApplyBonus;
            }
            else
            {
                OtherworldlyBonus bonus = new OtherworldlyBonus(50f, effectPrefab);
                EventManager.Instance.OnPlayerUnitDeath += bonus.ApplyBonus;
            }
        }

        while (animations.Count > 0)
        {
            var animation = animations.Pop();
            yield return animation;
        }
    }
}
