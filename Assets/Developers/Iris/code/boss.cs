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

    GameObject healthbar;
    ObjectPoolManager pooler;
    string prefabName;
    enemyManager enemyMan;

    private float damageTime = 0;
    private bool damaged = false;
    SpriteRenderer sr;

    public void takeDamage(int dmg)
    {
        health -= dmg;
        damaged = true;


        sr.color = new Color(0.7f, 0, 0);
        

        if (health <= 0)
        {
            enemyMan.decreaseEnemyCount();
            pooler.ReleaseObject(prefabName, gameObject);
            health = 0;
            healthbar.SetActive(false);
        }
        healthbar.GetComponent<Slider>().value = health;
    }

    public void InsantiateComponent(ref ObjectPoolManager objPooler, string prefName, ref enemyManager eneMan, ref Canvas UICanvas)
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        prefabName = prefName;
        pooler = objPooler;
        enemyMan = eneMan;
        damageTime = 0;

        healthbar = Instantiate(healthbarPrefab, UICanvas.transform);
        healthbar.GetComponent<RectTransform>().position = transform.position;
        healthbar.GetComponent<RectTransform>().Translate(new Vector3(Screen.width/2, Screen.height - 100, 0));

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
        if(damaged)
        {
            damageTime += Time.deltaTime;
            if (damageTime >= damageTimer)
            {
                damaged = false;
                damageTime = 0;
                sr.color = new Color(1, 1, 1);
            }
        }
    }
}
