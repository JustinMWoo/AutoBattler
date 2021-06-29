using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DefenceModifier 
{
    public int order;

    protected Unit unit;
    public abstract int Calculate(int damage);

}
