using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStart : State
{
    public override IEnumerator Start()
    {
        Board.Instance.SaveBoardState();

        // Start the event for combat starting
        yield return GameManager.Instance.StartCoroutine(EventManager.Instance.CombatStart());

        // Set the state to the start of the cycle
        TurnManager.Instance.SetState(new CycleStart());
    }
}
