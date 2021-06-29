using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Phase
{
    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator End()
    {
        yield break;
    }
}
