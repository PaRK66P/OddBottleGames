using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class attack1 : AttackClass
{
    public artileryAttack artileryPrefab;
    public int randomArtileryProjectileNo;
    ObjectPoolManager pooler;
    [Space]
    public int horizantalUnitsFromOrigin;
    public int verticalUnitsFromOrigin;
    public int originX;
    public int originY;

    float totalDelay;

    public override void Attack(ref bool b, ref List<int> itt, ref List<float> tim, ref ObjectPoolManager poolMan, ref GameObject callingObj)
    {
        totalDelay = artileryPrefab.delay + artileryPrefab.activeTime;
        pooler = poolMan;

        if (tim.Count() == 0)
        {
            tim.Add(0);//timer between waves
            itt.Add(0);//wave number

            for (int i = 0; i <= randomArtileryProjectileNo; ++i)
            {
                UnityEngine.Vector3 pos = new UnityEngine.Vector3(UnityEngine.Random.Range(-horizantalUnitsFromOrigin + originX, horizantalUnitsFromOrigin + originX), UnityEngine.Random.Range(-verticalUnitsFromOrigin + originY, verticalUnitsFromOrigin + originY), 0);
                UnityEngine.Vector3 rot = new UnityEngine.Vector3(0, 0, 0);

                GameObject obj = pooler.GetFreeObject(artileryPrefab.name);
                obj.GetComponent<artileryAttack>().InstantiateComponent(ref pooler, artileryPrefab.name, pos, rot);
            }
        }
        tim[0] += Time.deltaTime;

        if (tim[0] >= totalDelay)
        {
            b = false;
            itt.Clear();
            tim.Clear();
        }
    }
}
