using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class attackPaternsScript : MonoBehaviour
{
    public string pattern;
    List<List<int>> patList = new List<List<int>>();
    public float timeBetweenAttacks;
    float attackTimer;

    [Space]
    public List<AttackClass> attackList = new List<AttackClass>();
    
    int currentAttack;
    bool inAttack;
    List<int> itterators;
    

    // Start is called before the first frame update
    void Start()
    {
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
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!inAttack)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= timeBetweenAttacks)
            {
                inAttack = true;
            }
        }
        foreach (List<int> move in patList)
        {
            int i = 0;
            if (move.Count() > 1)
            {
                i = UnityEngine.Random.Range(0, move.Count() + 1);
            }
            attackList[move[i]].Attack(ref inAttack, ref itterators);
        }
    }
}
