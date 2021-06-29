using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPhase : Phase
{
    public override IEnumerator Start()
    {
        MapManager.Instance.OpenMap();
        ButtonManager.Instance.DisableButton(Buttons.Done);
        ButtonManager.Instance.DisableButton(Buttons.Map);
        ButtonManager.Instance.DisableButton(Buttons.Combat);
        yield break;
    }
    public override IEnumerator End()
    {
        MapManager.Instance.GetCurrentNode().NodeEvent.Execute();
        yield break;
    }
}