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

    float totalDelay;

    public override void Attack(ref bool b, ref List<int> itt, ref List<float> tim, ref ObjectPoolManager poolMan)
    {
        totalDelay = artileryPrefab.delay + artileryPrefab.activeTime;
        pooler = poolMan;

        if (tim.Count() == 0)
        {
            tim.Add(0);//timer between waves
            itt.Add(0);//wave number
        }

        tim[0] += Time.deltaTime;
        for (int i = 0; i <= randomArtileryProjectileNo; ++i)
        {
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(UnityEngine.Random.Range(-horizantalUnitsFromOrigin, horizantalUnitsFromOrigin), UnityEngine.Random.Range(-verticalUnitsFromOrigin, verticalUnitsFromOrigin), 0);
            UnityEngine.Vector3 rot = new UnityEngine.Vector3(0, 0, 0);

            GameObject obj = pooler.GetFreeObject(artileryPrefab.name);
            obj.GetComponent<artileryAttack>().InstantiateComponent(ref pooler, artileryPrefab.name, pos, rot);
        }

        if (tim[0] >= totalDelay)
        {
            b = false;
            itt.Clear();
            tim.Clear();
        }
    }
}
