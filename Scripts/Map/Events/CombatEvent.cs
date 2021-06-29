using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="MapEvent/Combat")]
public class CombatEvent : NodeEvent
{
    public override void Initiate(Node node)
    {
        CombatLayoutAssigner.Instance.AssignLayout(node);
    }

    public override void Execute()
    {
        GameManager.Instance.SetPhase(new CombatPhase());
    }
}
