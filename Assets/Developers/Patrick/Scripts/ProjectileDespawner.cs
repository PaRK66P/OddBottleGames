using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDespawner : MonoBehaviour
{
    [SerializeField]
    private LayerMask _despawnLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _despawnLayer) != 0)
        {
            Destroy(collision.gameObject);
        }
    }

}
