using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapEvent/Shop")]
public class ShopEvent : NodeEvent
{


    public override void Execute()
    {
        GameManager.Instance.SetPhase(new ShopPhase());
    }
}
