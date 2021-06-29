using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class NPCBoardState
{
    public List<NPCSavedUnit> units;
    public List<string> setNames;
    public List<int> setCounts;
    public List<string> tribeNames;
    public List<int> tribeCounts;

    public int difficulty;
}
