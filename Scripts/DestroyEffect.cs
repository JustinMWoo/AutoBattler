using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    public int destroyTime = 3;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, destroyTime);
    }
}
