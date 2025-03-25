using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IchorCircleAttack : AttackClass
{
    public GameObject smallProjectilePrefab;
    public GameObject largeProjectilePrefab;

    [Space]

    public float largeProjTravelTime = 0.4f;
    public float largeProjectileLifetime = 5;
    public float chargeTimer = 0.5f;
    public float timeBetweenWaves = 0.2f;

    //[Space]

    //public Vector3 spiralCenter; 

    [Space]

    public int projectilePerWaveNo = 6;
    public int numberOfWaves = 10;
    public int angleBetweenWaves = 5;

    [Space]

    public float largeProjectileSpeed = 4;
    public float smallProjectileSpeed = 15;

    private GameObject largeProjectile;

    public override void Attack(ref bool b, ref List<int> itt, ref List<float> tim, ref ObjectPoolManager pooler, ref GameObject callingObj)
    {
        if (tim.Count() == 0)
        {
            itt.Add(0);// [0] wave number

            tim.Add(chargeTimer);// [0] timer before waves start
            tim.Add(timeBetweenWaves);// [1] timer between waves

            largeProjectile = pooler.GetFreeObject(largeProjectilePrefab.name);
            largeProjectile.GetComponent<largeProjectileScript>().InstantiateComponent(ref pooler, largeProjectilePrefab.name, callingObj.transform.position, new Vector3(0, 0, 0), largeProjectileSpeed, largeProjTravelTime, largeProjectileLifetime);
        }

        if (tim[0] > 0)
        {
            tim[0] -= Time.deltaTime;
        }
        else
        {
            tim[1] -= Time.deltaTime;
            if (tim[1] <= 0)
            {
                tim[1] = timeBetweenWaves;
                float rotStep = 360 / projectilePerWaveNo;
                UnityEngine.Vector3 rotation = new UnityEngine.Vector3(0, 0, 0);
                rotation.z += angleBetweenWaves * itt[0];

                for (int i = 0; i < projectilePerWaveNo; i++)
                {
                    GameObject obj = pooler.GetFreeObject(smallProjectilePrefab.name);
                    obj.GetComponent<bossProjectile>().InstantiateComponent(ref pooler, smallProjectilePrefab.name, largeProjectile.transform.position, rotation);
                    rotation.z += rotStep;
                }
                itt[0]++;
            }
        }

        if (itt[0] == numberOfWaves)
        {
            b = false;
            itt.Clear();
            tim.Clear();
        }
    }
}
