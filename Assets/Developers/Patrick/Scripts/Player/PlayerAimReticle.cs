using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimReticle : MonoBehaviour
{
    [SerializeField]
    private GameObject ReticleImage;
    
    public void UpdateDirection(Vector2 direction)
    {
        float angle = Vector3.SignedAngle(Vector3.right, new Vector3(direction.x, direction.y, Vector3.right.z), new Vector3(0.0f, 0.0f, 1.0f));
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    public void SwitchToController()
    {
        ReticleImage.SetActive(true);
    }

    public void SwitchToMouse()
    {
        ReticleImage.SetActive(false);
    }
}
