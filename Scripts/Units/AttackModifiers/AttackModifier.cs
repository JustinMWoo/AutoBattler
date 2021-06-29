using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Class for special calculations that need to be done to calculate changes in attack
 */
public abstract class AttackModifier
{
    // The unit whose attack is being modified
    protected Unit unit;
    public abstract void Calculate(Unit target);

    public virtual void RemoveModifier()
    {
        unit.damage.RemoveAllModifiersFromSource(this);
        unit.defence.RemoveAllModifiersFromSource(this);
        unit.speed.RemoveAllModifiersFromSource(this);
        unit.maxHealth.RemoveAllModifiersFromSource(this);
    }
}
