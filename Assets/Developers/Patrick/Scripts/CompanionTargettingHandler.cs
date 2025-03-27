using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionTargettingHandler : MonoBehaviour
{
    private bool _hasCompanion = false;
    private CompanionManager _companionManager;

    public void InitialiseTargetting(ref CompanionManager companionManager)
    {
        _hasCompanion = true;
        _companionManager = companionManager;
        _companionManager.SetPlayerTarget(gameObject);
    }

    // Called when enemy dies
    public void ReleaseAsTarget()
    {
        if (!_hasCompanion)
        {
            return;
        }

        _companionManager.RemovePlayerTarget(gameObject);
    }
}
