using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class attack2 : AttackClass
{
    public GameObject artileryPrefab;
    ObjectPoolManager pooler;
    public int targetedArtileryProjectileNo = 5;
    public float timeBetweenStrikes = 0.5f;

    public override void Attack(ref bool b, ref List<int> itt, ref List<float> tim, ref ObjectPoolManager poolMan, ref GameObject callingObj)
    {
        pooler = poolMan;
        if(tim.Count() == 0)
        {
            tim.Add(0);
            itt.Add(0);
        }

        tim[0] += Time.deltaTime;
        if (tim[0] >= timeBetweenStrikes)
        {
            tim[0] = 0;
            if (itt[0] < 5)
            {
                UnityEngine.Vector3 pos = new UnityEngine.Vector3(0, 0, 0);

                if (GameObject.FindGameObjectWithTag("Player"))
                {
                    pos = GameObject.FindGameObjectWithTag("Player").transform.position;
                }
                UnityEngine.Vector3 rot = new UnityEngine.Vector3(0, 0, 0);

                GameObject obj = pooler.GetFreeObject(artileryPrefab.name);
                obj.GetComponent<artileryAttack>().InstantiateComponent(ref pooler, artileryPrefab.name, pos, rot);
                obj.transform.position = pos;
                obj.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 0);

                itt[0]++;
            }
            else
            {
                b = false;
                itt.Clear();
                tim.Clear();
            }
        }
    }
}
