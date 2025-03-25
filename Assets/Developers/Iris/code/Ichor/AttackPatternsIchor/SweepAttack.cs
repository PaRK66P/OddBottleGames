using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SweepAttack : AttackClass
{
    public GameObject projectilePrefab;

    public int projectileNo = 10;
    public float chargeUpTimer = 0.5f;
    public float projectileSpeed = 20;

    public int waves = 2;
    public float timeBetweenWaves = 0.5f;
    public float waveLifespan = 2;

    public float TopLimit;
    public float BottomLimit;
    public float LeftLimit;

    public bool collideWithWalls = false;

    public override void Attack(ref bool b, ref List<int> itt, ref List<float> tim, ref ObjectPoolManager dPooler, ref GameObject callingObj)
    {
        if (tim.Count() == 0)
        {
            tim.Add(0); // [0] timer beween wave spawns
            tim.Add(waveLifespan); // [0] timer after last wave spawn
            itt.Add(0); // [0] waves spawned
        }

        if (itt[0] < waves)
        {
            tim[0] -= Time.deltaTime;
            if (tim[0] <= 0)
            {
                tim[0] = timeBetweenWaves;

                UnityEngine.Vector3 pos = new UnityEngine.Vector3(LeftLimit, TopLimit, 0);
                UnityEngine.Vector3 rotation = new UnityEngine.Vector3(0, 0, 0);

                //spawn a wave
                for (int i = 0; i < projectileNo; i++)
                {
                    GameObject obj = dPooler.GetFreeObject(projectilePrefab.name);
                    obj.GetComponent<SweepProjectile>().InstantiateComponent(ref dPooler, projectilePrefab.name, pos, rotation, projectileSpeed, chargeUpTimer, waveLifespan, collideWithWalls);

                    pos.y -= (TopLimit - BottomLimit) / projectileNo;
                }

                itt[0]++;
            }
        }
        else
        {
            tim[1] -= Time.deltaTime;
            if(tim[1] <= 0)
            {
                b = false;
                itt.Clear();
                tim.Clear();
            }
        }
    }
}
