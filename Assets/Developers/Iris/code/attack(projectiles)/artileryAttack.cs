using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class artileryAttack : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite damageSprite;
    public GameObject imageRef;

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
            GetComponentInChildren<SpriteRenderer>().sprite = damageSprite;
            imageRef.transform.localPosition = new Vector3(0, 0.5f, 0);
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
            Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x - transform.position.x,
                    collision.gameObject.transform.position.y - transform.position.y);
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized, 1, 5);
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
        imageRef.transform.localPosition = new Vector3(0, 0, 0);
        GetComponentInChildren<SpriteRenderer>().sprite = normalSprite;
    }
}
