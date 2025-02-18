using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackClass : MonoBehaviour
{
    public virtual void Attack(ref bool b, ref List<int> itt, ref List<float> tim, ref ObjectPoolManager poolMan, ref GameObject callingObj) { }
}
