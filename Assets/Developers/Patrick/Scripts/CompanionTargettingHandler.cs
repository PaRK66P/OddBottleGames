using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionTargettingHandler : MonoBehaviour
{
    private bool _hasCompanion = false;
    private CompanionManager _companionManager;

    public void InitialiseTargetting(ref CompanionManager companionManager)
    {
        Debug.Log("Add target: " + gameObject.name);
        _hasCompanion = true;
        _companionManager = companionManager;
        _companionManager.SetPlayerTarget(gameObject);
    }

    // Called when enemy dies
    public void ReleaseAsTarget()
    {
        if (!_hasCompanion)
        {
            Debug.Log("Called but no release needed: " + gameObject.name);
            return;
        }

        Debug.Log("Remove target: " + gameObject.name);

        _companionManager.RemovePlayerTarget(gameObject);
    }
}
