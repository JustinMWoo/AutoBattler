using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurn : State
{
    public override IEnumerator Start()
    {
        // Event for end of turn
        yield return GameManager.Instance.StartCoroutine(EventManager.Instance.EndTurn());

        // Subscribe units the the events for its turn
        TurnManager.Instance.GetCurrentUnit().RemoveFromEvents();

        // At the end of turn if one of the teams has no units left then end combat
        if (Board.Instance.PlayerUnitCount() == 0 || Board.Instance.NPCUnitCount() == 0)
        {
            TurnManager.Instance.SetState(new EndCombat());
        }
        // Set the current unit to the next in the queue and check if queue is empty
        else if (TurnManager.Instance.NextUnit())
        {
            // Start the turn of the new unit
            TurnManager.Instance.SetState(new TurnStart());
        }
        else
        {
            // Start a new cycle because there are no units left in the queue
            TurnManager.Instance.SetState(new CycleStart());
        }
    }
}