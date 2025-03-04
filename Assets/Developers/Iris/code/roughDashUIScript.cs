using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roughDashUIScript : MonoBehaviour
{
    public PlayerData playerData;
    public List<GameObject> dashUI;
    public GameObject dashUIPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //Player player = GameObject.FindWithTag("Player");
        //GameObject 
        //playerData = player. 
    }

    // Update is called once per frame
    void Update()
    {
        switch(playerData.numberOfDashCharges)
        {
            case 0:
                for(int i =0; i<3; i++)
                {
                    dashUI[i].SetActive(false);
                }
                break;
            case 1:
                dashUI[0].SetActive(true);
                for (int i = 0; i < 2; i++)
                {
                    dashUI[i].SetActive(false);
                }
                break;
            case 2:
                dashUI[2].SetActive(false);
                for (int i = 0; i < 2; i++)
                {
                    dashUI[i].SetActive(true);
                }
                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    dashUI[i].SetActive(true);
                }
                break;
            default:
                break;
        }
    }
}
