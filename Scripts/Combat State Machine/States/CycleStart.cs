using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleStart : State
{
    public override IEnumerator Start()
    {
        // Event for start of cycle
        yield return GameManager.Instance.StartCoroutine(EventManager.Instance.CycleStart());

        // Use speeds post buffs for sorting
        TurnManager.Instance.SortBySpeed();

        // Set the current unit in the turn manager
        TurnManager.Instance.NextUnit();

        TurnManager.Instance.SetState(new TurnStart());
    }
}
