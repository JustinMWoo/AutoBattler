using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NPCSavedUnit 
{
    public string name;
    public int boardX, boardZ;

    public NPCSavedUnit(int boardX, int boardZ, string name)
    {
        this.name = name;
        this.boardX = boardX;
        this.boardZ = boardZ;
    }
}
