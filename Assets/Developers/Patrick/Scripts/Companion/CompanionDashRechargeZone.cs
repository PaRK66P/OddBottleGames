using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionDashRechargeZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) // Might be better as TriggerStay
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("Recharge");
            //collision.gameObject.GetComponent<PlayerManager>().GainDashCharge();
        }
    }
}
