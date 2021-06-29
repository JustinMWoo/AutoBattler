using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeEvent : ScriptableObject
{
    public string Name;
    public Sprite icon;
    public int spawnWeight;
    public virtual void Initiate(Node node)
    {

    }

    public abstract void Execute();
}
