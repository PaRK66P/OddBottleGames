using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class attackPaternsScript : MonoBehaviour
{
    public int phaseNo;
    public string pattern;
    List<List<int>> patList = new List<List<int>>();
    public float timeBetweenAttacks;

    [Space]
    public List<AttackClass> attackList = new List<AttackClass>();
    public bool stunned = false;

    private int currentAttack = 0;
    private int attackItterator = 0;
    private bool inAttack = new bool();
    private List<int> itterators = new List<int>();
    private List<float> timers = new List<float>();
    private float attackTimer;

    private ObjectPoolManager pooler;

    private GameObject myself;

    public void InstantiateComponent(ref ObjectPoolManager objPooler)
    {
        pooler = objPooler;
    }

    // Start is called before the first frame update
    void Start()
    {
        inAttack = false;

        List<int> noVec = new List<int>();

        int n = 0;
        foreach (char c in pattern)
        {
            
            if (char.IsNumber(c))
            {
                string x = c.ToString();
                if (n!=0)
                {
                    n *= 10;
                }
                n += Convert.ToInt32(x);
            }
            else if(c == ',')
            {
                noVec.Add(n);
                patList.Add(noVec);
                noVec = new List<int>();
                n = 0;
            }
            else if (c == '/')
            {
                noVec.Add(n);
                n = 0;
            }
            else
            {
                break;
            }
        }

        n = 0;
        if (patList[0].Count() > 1)
        {
            n = UnityEngine.Random.Range(0, patList[0].Count());
            currentAttack = patList[0][n];
        }
        currentAttack = patList[0][n];

        myself = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stunned)
        {
            if (!inAttack)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= timeBetweenAttacks)
                {
                    inAttack = true;
                    attackTimer = 0;
                    int n = 0;
                    if (patList[attackItterator].Count() > 1)
                    {
                        n = UnityEngine.Random.Range(0, patList[attackItterator].Count());
                        currentAttack = patList[attackItterator][n];
                    }
                    currentAttack = patList[attackItterator][n];
                    attackItterator++;
                }
            }
            else
            {
                attackList[currentAttack].Attack(ref inAttack, ref itterators, ref timers, ref pooler, ref myself);
            }

            if (attackItterator == patList.Count())
            {
                attackItterator = 0;
            }
        }
    }
}
