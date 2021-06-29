using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : State
{
    public override IEnumerator Start()
    {
        // Event for start of attack depending on owner of unit
        if (TurnManager.Instance.GetCurrentUnit().IsPlayerUnit())
        {
            yield return GameManager.Instance.StartCoroutine(EventManager.Instance.PlayerAttack());
        }
        else
        {
            yield return GameManager.Instance.StartCoroutine(EventManager.Instance.NPCAttack());
        }

        yield return new WaitForSeconds(0.5f);

        TurnManager.Instance.SetState(new EndTurn());
    }
}