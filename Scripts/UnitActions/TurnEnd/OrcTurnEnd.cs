using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcTurnEnd : UnitAction
{
    public int Heal { get; set; }
    [SerializeField]
    private GameObject healEffectPrefab;

    public override IEnumerator Execute()
    {
        unit.Heal(Heal);
        Instantiate(healEffectPrefab, unit.transform);
        yield return BonusManager.Instance.StartCoroutine(FloatingStatChangeSpawner.Instance.SpawnFloatingIcon(unit.transform, Heal.ToString(), Stat.health, true));
    }

    // Remove this component at the end of combat
    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.OnEndCombat += RemoveBonus;

    }
    private IEnumerator RemoveBonus()
    {
        Destroy(this.gameObject);
        yield return null;
    }
    private void OnDestroy()
    {
        EventManager.Instance.OnEndCombat -= RemoveBonus;
    }
}
