using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;
    public void SpawnProjectile()
    {
        Instantiate(projectilePrefab, transform.position, transform.rotation);
    }
}
