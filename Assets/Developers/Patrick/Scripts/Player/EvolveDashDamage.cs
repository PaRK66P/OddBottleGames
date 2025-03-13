using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveDashDamage : MonoBehaviour
{
    private PlayerData _playerData;

    public void InitialiseScript(ref PlayerData playerData)
    {
        _playerData = playerData;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(((1 << collision.gameObject.layer) & _playerData.enemyLayers) != 0)
        {
            if (collision.gameObject.tag == "Boss")
            {
                collision.gameObject.GetComponent<boss>().takeDamage((int) _playerData.evolvedDashDamage);
            }
            else if (collision.gameObject.GetComponent<AISimpleBehaviour>() != null)
            {
                collision.gameObject.GetComponent<AISimpleBehaviour>().TakeDamage(_playerData.evolvedDashDamage, gameObject.transform.position - collision.gameObject.transform.position);
            }
        }
    }
}
