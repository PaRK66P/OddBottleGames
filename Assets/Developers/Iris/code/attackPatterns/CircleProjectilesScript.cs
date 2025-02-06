using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleProjectilesScript : AttackClass
{
    public GameObject projectilePrefab;
    ObjectPoolManager pooler;
    public int projectileAttackNo = 10;    

    public override void Attack(ref bool b, ref List<int> itt, ref List<float> tim, ref ObjectPoolManager poolMan)
    {
        pooler = poolMan;
        float rotStep = 360 / projectileAttackNo;
        UnityEngine.Vector3 rotation = new UnityEngine.Vector3(0, 0, 0);

        for (int i = 0; i < projectileAttackNo; i++)
        {
            GameObject obj = pooler.GetFreeObject(projectilePrefab.name);
            obj.GetComponent<bossProjectile>().InstantiateComponent(ref pooler, projectilePrefab.name);
            obj.transform.position = transform.position;
            obj.transform.rotation = UnityEngine.Quaternion.Euler(rotation);
            rotation.z += rotStep;
        }
        b = false;
        itt.Clear();
        tim.Clear();
    }
}
