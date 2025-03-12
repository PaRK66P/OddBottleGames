using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnvironmentBehaviour : MonoBehaviour
{
    [SerializeField]
    private ProjectileBehaviour projectileBehaviour;

    [SerializeField]
    private LayerMask environment;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & environment) != 0)
        {
            projectileBehaviour.SetToRelease();
        }
    }
}
