using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State state;

    public void SetState(State state)
    {
        this.state = state;
        if (this.state != null)
        {
            Debug.Log("Starting state..." + state);
            StartCoroutine(state.Start());
        }
        else
        {

        }
    }
}
