using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionDetection : MonoBehaviour
{
    private CompanionFriendData _dataObj;

    private List<GameObject> _targetsList;

    public void InitialiseComponent(ref CompanionFriendData dataObj)
    {
        _dataObj = dataObj;

        _targetsList = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _dataObj.enemyLayer) != 0)
        {
            AddTarget(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _dataObj.enemyLayer) != 0)
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

    public GameObject GetTarget()
    {
        if (_targetsList == null || _targetsList.Count == 0) { return null; }

        GameObject closestTarget = _targetsList[0];
        float closestDistance = (_targetsList[0].transform.position - transform.position).sqrMagnitude;
        float distance;

        foreach (GameObject target in _targetsList)
        {
            distance = (target.transform.position - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestTarget = target;
                closestDistance = distance;
            }
        }

        return closestTarget;
    }
}
