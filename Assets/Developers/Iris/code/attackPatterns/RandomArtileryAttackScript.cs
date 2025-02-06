using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack1 : AttackClass
{
    public GameObject artileryPrefab;
    public int randomArtileryProjectileNo;
    ObjectPoolManager pooler;
    [Space]
    public int horizantalUnitsFromOrigin;
    public int verticalUnitsFromOrigin;

    public override void Attack(ref bool b, ref List<int> itt, ref List<float> tim, ref ObjectPoolManager poolMan)
    {
        pooler = poolMan;
        for (int i = 0; i <= randomArtileryProjectileNo; ++i)
        {
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(UnityEngine.Random.Range(-horizantalUnitsFromOrigin, horizantalUnitsFromOrigin), UnityEngine.Random.Range(-verticalUnitsFromOrigin, verticalUnitsFromOrigin), 0);

            GameObject obj = pooler.GetFreeObject(artileryPrefab.name);
            obj.GetComponent<artileryAttack>().InstantiateComponent(ref pooler, artileryPrefab.name);
            obj.transform.position = pos;
            obj.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, 0);
    }

        b = false;
        itt.Clear();
        tim.Clear();
    }
}
