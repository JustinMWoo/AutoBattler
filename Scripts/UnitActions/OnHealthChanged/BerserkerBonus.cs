using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerBonus : UnitAction
{
    public int MaxDamage { get; set; }

    public override IEnumerator Execute()
    {
        // Remove damage mods from this object if they exist
        unit.damage.RemoveAllModifiersFromSource(this);

        // Calculate how much damage to add
        int value = Mathf.RoundToInt(MaxDamage * (1 - (unit.GetCurrentHealth() / unit.maxHealth.Value)));

        unit.damage.AddModifier(new StatModifier(value, StatModType.Flat, this));
        yield return null;
    }

    // Remove this component at the end of combat
    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.OnEndCombat += RemoveBonus;

    }
    private IEnumerator RemoveBonus()
    {
        unit.RemoveHealthChanged(this);
        Destroy(this.gameObject);
        yield return null;
    }
    private void OnDestroy()
    {
        EventManager.Instance.OnEndCombat -= RemoveBonus;
    }
}
