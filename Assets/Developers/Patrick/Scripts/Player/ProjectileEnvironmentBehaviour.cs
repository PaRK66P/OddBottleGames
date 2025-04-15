using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnvironmentBehaviour : MonoBehaviour
{
    [SerializeField]
    private ProjectileBehaviour _projectileBehaviour;

    [SerializeField]
    private LayerMask _environment;

    // Releases the projectile from the object pooler on collision with the environment
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _environment) != 0)
        {
            _projectileBehaviour.SetToRelease();
        }
    }
}
