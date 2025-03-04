using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class boss : MonoBehaviour
{
    public int health = 100;
    public GameObject healthbarPrefab;
    public float damageTimer = 0.15f;

    public GameObject deathBackdropPrefab;
    public float deathSequenceTime = 2.0f;

    Canvas UICanvas;
    GameObject healthbar;
    ObjectPoolManager pooler;
    string prefabName;
    enemyManager enemyMan;
    GameObject deathBackdrop;
    float deathSequenceTimer = 0.0f;
    bool dead = false;

    private float damageTime = 0;
    private bool damaged = false;
    SpriteRenderer sr;

    public void takeDamage(int dmg)
    {
        health -= dmg;
        damaged = true;


        sr.color = new Color(1.0f, 0.2f, 0.2f);
        

        if (health <= 0)
        {
            enemyMan.decreaseEnemyCount();
            
            health = 0;
            healthbar.SetActive(false);
            dead = true;
            sr.color = new Color(0, 0, 0);

            //deathBackdrop = Instantiate(deathBackdropPrefab, UICanvas.transform);
            //deathBackdrop.GetComponentInChildren<RectTransform>().localPosition = transform.position;


            //deathBackdrop.GetComponent<RectTransform>().Translate(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            //deadSprite = Instantiate(deadSpritePrefab, UICanvas.transform);
            //deadSprite.GetComponent<RectTransform>().position = transform.position;

            //pooler.ReleaseObject(prefabName, gameObject);

            GetComponent<attackPaternsScript>().stunned = true;
        }
        healthbar.GetComponent<Slider>().value = health;
    }

    public void InsantiateComponent(ref ObjectPoolManager objPooler, string prefName, ref enemyManager eneMan, ref Canvas dUICanvas)
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        prefabName = prefName;
        pooler = objPooler;
        enemyMan = eneMan;
        UICanvas = dUICanvas;
        damageTime = 0;

        healthbar = Instantiate(healthbarPrefab, dUICanvas.transform);
        //healthbar.GetComponent<RectTransform>().position = transform.position;
        healthbar.GetComponent<RectTransform>().Translate(new Vector3(Screen.width / 2 - 400, Screen.height - 240, 0));

        healthbar.GetComponent<Slider>().maxValue = health;
        healthbar.GetComponent<Slider>().value = health;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(damaged && !dead)
        {
            damageTime += Time.deltaTime;
            if (damageTime >= damageTimer)
            {
                damaged = false;
                damageTime = 0;
                sr.color = new Color(1, 1, 1);
            }
        }
        if (dead)
        {
            deathSequenceTimer += Time.deltaTime;
            if (deathSequenceTimer >= deathSequenceTime)
            {
                pooler.ReleaseObject(prefabName, gameObject);
            }
        }
    }
}
