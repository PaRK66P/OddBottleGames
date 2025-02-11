using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class artileryAttack : MonoBehaviour
{
    public float delay = 1;
    public float activeTime = 1;
    ObjectPoolManager pooler;
    string prefabName;

    float timeElapsed = 0;
    bool damage = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed >= delay)
        {
            changeColor(new Color(1, 0, 0, 1));
            gameObject.layer = LayerMask.NameToLayer("EnemyAttack");
            damage = true;
        }

        if(timeElapsed >= activeTime + delay)
        {
            pooler.ReleaseObject(prefabName, gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && damage)
        {
            //implement player damage
            Debug.Log("player took damage");
        }
    }

    public void InstantiateComponent(ref ObjectPoolManager poolMan, string prefName, Vector3 pos, Vector3 rot)
    {
        pooler = poolMan;
        prefabName = prefName;
        transform.position = pos;
        transform.rotation = UnityEngine.Quaternion.Euler(rot);
        timeElapsed = 0;
        damage = false;
        changeColor(new Color(1, 1, 0, 1));
    }

    void changeColor(Color c)
    {
        GetComponent<SpriteRenderer>().color = c;
    }
}
