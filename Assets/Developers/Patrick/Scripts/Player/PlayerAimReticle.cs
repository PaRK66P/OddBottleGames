using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimReticle : MonoBehaviour
{
    [SerializeField]
    private GameObject _reticleImage;
    
    // Updates direction of reticle
    public void UpdateDirection(Vector2 direction)
    {
        // By default reticle is facing right of player (1, 0)
        float angle = Vector3.SignedAngle(Vector3.right, new Vector3(direction.x, direction.y, Vector3.right.z), new Vector3(0.0f, 0.0f, 1.0f));
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    // Don't need the reticle with mouse as the mouse is the reticle
    #region Device Management
    public void SwitchToController()
    {
        _reticleImage.SetActive(true);
    }

    public void SwitchToMouse()
    {
        _reticleImage.SetActive(false);
    }
    #endregion
}
