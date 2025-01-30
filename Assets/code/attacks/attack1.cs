using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack1 : AttackClass
{
    public override void Attack(ref bool b, ref List<int> itt)
    {
        Debug.Log("attack1");
        b = true;
    }
}
