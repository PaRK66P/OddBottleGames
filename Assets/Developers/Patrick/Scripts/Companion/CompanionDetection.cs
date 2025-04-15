using System.Collections.Generic;
using UnityEngine;

public class CompanionDetection : MonoBehaviour
{
    // Data
    private CompanionFriendData _dataObj;

    // Values
    private List<GameObject> _targetsList;

    public void InitialiseComponent(ref CompanionFriendData dataObj)
    {
        _dataObj = dataObj;

        _targetsList = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _dataObj.EnemyLayer) != 0)
        {
            AddTarget(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _dataObj.EnemyLayer) != 0)
        {
            RemoveTarget(collision.gameObject);
        }
    }

    private void AddTarget(GameObject target)
    {
        _targetsList.Add(target);
    }

    private void RemoveTarget(GameObject target)
    {
        _targetsList.Remove(target);
    }

    // Returns the closest game object of its current targets
    public GameObject GetTarget()
    {
        if (_targetsList == null || _targetsList.Count == 0) { return null; } // No targets to check

        GameObject closestTarget = _targetsList[0]; // Set first target as the one to compare
        float closestDistance = (_targetsList[0].transform.position - transform.position).sqrMagnitude;
        float distance;

        foreach (GameObject target in _targetsList) // Iterate through each target
        {
            distance = (target.transform.position - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                // If the current target is closer than the tracked closest one then replace it with the current target
                closestTarget = target;
                closestDistance = distance;
            }
        }

        return closestTarget;
    }
}
