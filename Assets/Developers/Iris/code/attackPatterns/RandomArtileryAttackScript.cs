using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class attack1 : AttackClass
{
    public GameObject artileryPrefab;
    public int randomArtileryProjectileNo;
    ObjectPoolManager pooler;
    [Space]
    public float horizantalUnitsFromOrigin;
    public float verticalUnitsFromOrigin;
    public float originX;
    public float originY;

    float leftB;
    float rightB;
    float upperB;
    float lowerB;

    float totalDelay;
    List<float> xCoords;
    List<bool> xBoolStat;
    List<float> yCoords;
    List<bool> yBoolStat;

    int noOfCoords;

    //public void Start()
    //{
    //    leftB = originX - horizantalUnitsFromOrigin;
    //    rightB = originX + horizantalUnitsFromOrigin;
    //    upperB = originY + verticalUnitsFromOrigin;
    //    lowerB = originY - verticalUnitsFromOrigin;

    //    //Debug.Log(leftB);
    //    //Debug.Log(rightB);
    //    //Debug.Log(upperB);
    //    //Debug.Log(lowerB);

    //    noOfCoords = randomArtileryProjectileNo;
    //    float xStep = (rightB - leftB) / noOfCoords;
    //    float yStep = (upperB - lowerB) / noOfCoords;

    //    Debug.Log(xStep);
    //    //Debug.Log(yStep);

    //    xCoords.Clear();
    //    yCoords.Clear();
    //    xBoolStat.Clear();
    //    yBoolStat.Clear();

    //    for (int i = 0; i <= noOfCoords; ++i)
    //    {
    //        xCoords.Add(leftB + xStep * i);
    //        yCoords.Add(lowerB + yStep * i);
    //        //Debug.Log(xCoords[i]);
    //        //Debug.Log(yCoords[i]);
    //        xBoolStat.Add(false);
    //        yBoolStat.Add(false);
    //    }
    //}

    public override void Attack(ref bool b, ref List<int> itt, ref List<float> tim, ref ObjectPoolManager poolMan, ref GameObject callingObj)
    {
        totalDelay = artileryPrefab.GetComponent<artileryAttack>().delay + artileryPrefab.GetComponent<artileryAttack>().activeTime;
        pooler = poolMan;

        leftB = originX - horizantalUnitsFromOrigin;
        rightB = originX + horizantalUnitsFromOrigin;
        upperB = originY + verticalUnitsFromOrigin;
        lowerB = originY - verticalUnitsFromOrigin;

        //Debug.Log(leftB);
        //Debug.Log(rightB);
        //Debug.Log(upperB);
        //Debug.Log(lowerB);

        noOfCoords = randomArtileryProjectileNo / 3;
        float xStep = (rightB - leftB) / noOfCoords;
        float yStep = (upperB - lowerB) / noOfCoords;

        //Debug.Log(xStep);
        //Debug.Log(yStep);

        xCoords.Clear();
        yCoords.Clear();
        xBoolStat.Clear();
        yBoolStat.Clear();

        for (int i = 0; i <= noOfCoords; ++i)
        {
            xCoords.Add(leftB + xStep * i);
            yCoords.Add(lowerB + yStep * i);
            //Debug.Log(xCoords[i]);
            //Debug.Log(yCoords[i]);
            xBoolStat.Add(false);
            yBoolStat.Add(false);
        }

        if (tim.Count() == 0)
        {
            tim.Add(0);//timer between waves
            itt.Add(0);//wave number

            for (int i = 0; i <= randomArtileryProjectileNo; ++i)
            {
                int x = UnityEngine.Random.Range(0, noOfCoords);
                int y = UnityEngine.Random.Range(0, noOfCoords);

                for (int n = 0; n<10;++n)
                {
                    if (xBoolStat[x] && yBoolStat[y])
                    {
                        int c = UnityEngine.Random.Range(0, 2);
                        if(c == 0)
                        {
                            x++;
                            if(x >= noOfCoords)
                            {
                                x = 0;
                            }
                        }
                        else
                        {
                            y++;
                            if (y >= noOfCoords)
                            {
                                y = 0;
                            }
                        }
                    }
                }

                xBoolStat[x] = true;
                yBoolStat[y] = true;

                UnityEngine.Vector3 pos = new UnityEngine.Vector3(xCoords[x], yCoords[y], 0);
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
