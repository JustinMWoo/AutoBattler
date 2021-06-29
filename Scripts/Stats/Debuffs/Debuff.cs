using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Debuff
{
    protected int maxDuration;
    protected int turnsLeft;
    protected Unit unit;

    public Debuff(Unit unit, int max)
    {
        this.unit = unit;
        maxDuration = max;
        turnsLeft = max;
    }

    public virtual IEnumerator Execute()
    {
        turnsLeft -= 1;
        //Debug.Log(turnsLeft);
        if (turnsLeft < 1)
        {
            TimedDebuffManager.Instance.RemoveDebuff(unit, this);
        }

        yield return new WaitForSeconds(0.5f);
    }

    public void RefreshTime()
    {
        turnsLeft = maxDuration;
    }
}
