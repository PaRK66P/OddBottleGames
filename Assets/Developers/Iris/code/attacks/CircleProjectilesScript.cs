using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleProjectilesScript : AttackClass
{
    public GameObject projectilePrefab;
    public int projectileAttackNo = 10;

    public override void Attack(ref bool b, ref List<int> itt, ref List<float> tim)
    {
        float rotStep = 360 / projectileAttackNo;
        UnityEngine.Vector3 rotation = new UnityEngine.Vector3(0, 0, 0);

        for (int i = 0; i < projectileAttackNo; i++)
        {
            Instantiate(projectilePrefab, gameObject.transform.position, UnityEngine.Quaternion.Euler(rotation));
            rotation.z += rotStep;
        }
        b = false;
        itt.Clear();
        tim.Clear();
    }
}
