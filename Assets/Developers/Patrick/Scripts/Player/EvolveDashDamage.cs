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
        if(((1 << collision.gameObject.layer) & _playerData.EnemyLayers) != 0)
        {
            if (collision.gameObject.tag == "Boss") // Damage boss
            {
                collision.gameObject.GetComponent<boss>().takeDamage((int) _playerData.EvolvedDashDamage);
            }
            else if (collision.gameObject.GetComponent<AISimpleBehaviour>() != null) // Damage basic enemies
            {
                collision.gameObject.GetComponent<AISimpleBehaviour>().TakeDamage(_playerData.EvolvedDashDamage, gameObject.transform.position - collision.gameObject.transform.position);
            }
        }
    }
}
