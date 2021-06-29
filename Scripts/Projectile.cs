using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5;
    public float destroyTime = 3;

    private void Start()
    {
        Destroy(this.gameObject, destroyTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
