using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tribes/Void")]
public class VoidTribe : BaseTribe
{
    private void Awake()
    {
        tierRequirements = new List<int> { 2, 4 };
    }
    [SerializeField]
    private GameObject StunPrefab;
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
                AttackModifier mod = new VoidModifier(unit, 10, StunPrefab);
                unit.AddAttackModifier(mod);

                animations.Push(BonusManager.Instance.StartCoroutine(SpawnBuffPrefab(unit.transform)));
            }
        }
        else
        {
            foreach (Unit unit in units)
            {
                AttackModifier mod = new VoidModifier(unit, 20, StunPrefab);
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
