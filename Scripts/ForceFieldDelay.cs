using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldDelay : MonoBehaviour
{
    public float time;
    private ParticleSystem ps;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (time <= 0)
        {
            var externalForces = ps.externalForces;
            externalForces.enabled = true;
            Destroy(this);
        }
        else
        {
            time -= Time.deltaTime;
        }
    }
}
