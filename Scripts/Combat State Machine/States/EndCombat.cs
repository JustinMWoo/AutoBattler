using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCombat : State
{
    public override IEnumerator Start()
    {
        // Run event for end of combat
        yield return GameManager.Instance.StartCoroutine(EventManager.Instance.EndCombat());

        // Return state to start of combat
        TurnManager.Instance.SetState(null);

        GameManager.Instance.EndCurrent();
    }
}