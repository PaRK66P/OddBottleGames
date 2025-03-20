using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SweepAttack : AttackClass
{
    public GameObject projectilePrefab;

    public int projectileNo = 10;
    public float chargeUpTimer = 0.5f;

    public int waves = 2;
    public float timeBetweenWaves = 0.5f;

    public float TopLimit;
    public float BottomLimit;
    public float LeftLimit;
    public float RightLimit;

    public override void Attack(ref bool b, ref List<int> itt, ref List<float> tim, ref ObjectPoolManager poolMan, ref GameObject callingObj)
    {
        if (tim.Count() == 0)
        {
            tim.Add(0);
            itt.Add(0); //[0] attack is finished
            itt.Add(0); //[1] charge timer finished
            itt.Add(0); //[2] waves spawned
        }

        if (itt[2] < waves)
        {
            tim[0] -= Time.deltaTime;
            if (tim[0] <= 0)
            {
                tim[0] = timeBetweenWaves;
                itt[1] = 1;
                spawnWave();
                itt[2]++;
            }
        }
        


        if (itt[0] == 1)
        {
            b = false;
            itt.Clear();
            tim.Clear();
        }
    }

    private void spawnWave()
    {

    }
}
