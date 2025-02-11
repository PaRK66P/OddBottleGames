using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpiralAttack : AttackClass
{
    public GameObject projectilePrefab;
    ObjectPoolManager pooler;
    public int projectilePerWaveNo = 6;
    public int numberOfWaves = 10;
    public float timeBetweenWaves = 0.2f;
    public int angleBetweenWaves = 5;

    public override void Attack(ref bool b, ref List<int> itt, ref List<float> tim, ref ObjectPoolManager poolMan)
    {
        pooler = poolMan;
        if (tim.Count() == 0)
        {
            tim.Add(0);//timer between waves
            itt.Add(0);//wave number
        }

        tim[0] += Time.deltaTime;
        if (tim[0]>=timeBetweenWaves)
        {
            tim[0] = 0;
            float rotStep = 360 / projectilePerWaveNo;
            UnityEngine.Vector3 rotation = new UnityEngine.Vector3(0, 0, 0);
            rotation.z += angleBetweenWaves * itt[0];

            for (int i = 0; i < projectilePerWaveNo; i++)
            {
                GameObject obj = pooler.GetFreeObject(projectilePrefab.name);
                obj.GetComponent<bossProjectile>().InstantiateComponent(ref pooler, projectilePrefab.name, transform.position, rotation);
                rotation.z += rotStep;
            }
            itt[0]++;
        }

        if (itt[0] == numberOfWaves)
        {
            b = false;
            itt.Clear();
            tim.Clear();
        }
    }
}