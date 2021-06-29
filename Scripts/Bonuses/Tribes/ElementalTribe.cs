using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tribes/Elemental")]
public class ElementalTribe : BaseTribe
{
    [SerializeField]
    private GameObject ElementalBonusPrefab;

    private void Awake()
    {
        tierRequirements = new List<int> { 2, 4, 6 };
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
                    ElementalBonus action = Instantiate(ElementalBonusPrefab, unit.gameObject.transform).GetComponent<ElementalBonus>();
                    action.MaxSpawns = 2;
                    unit.AddOnDeath(action);
                }
            }
        }
        else if (count < tierRequirements[2])
        {
            foreach (Unit unit in units)
            {
                if (unit.Tribe == this)
                {
                    animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
                    ElementalBonus action = Instantiate(ElementalBonusPrefab, unit.gameObject.transform).GetComponent<ElementalBonus>();
                    action.MaxSpawns = 2;
                    unit.AddOnDeath(action);
                }
            }
        }
        else
        {
            foreach (Unit unit in units)
            {
                if (unit.Tribe == this)
                {
                    animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
                    ElementalBonus action = Instantiate(ElementalBonusPrefab, unit.gameObject.transform).GetComponent<ElementalBonus>();
                    action.MaxSpawns = 3;
                    unit.AddOnDeath(action);
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
