using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnStart : State
{
    public override IEnumerator Start()
    {
        Debug.Log("Starting turn for " + TurnManager.Instance.GetCurrentUnit().UnitData.name + " player unit: " + TurnManager.Instance.GetCurrentUnit().IsPlayerUnit());

        // Subscribe units the the events for its turn
        TurnManager.Instance.GetCurrentUnit().SubscribeToEvents();

        // Event for start of turn
        yield return GameManager.Instance.StartCoroutine(EventManager.Instance.TurnStart());

        TurnManager.Instance.SetState(new Attack());
    }
}