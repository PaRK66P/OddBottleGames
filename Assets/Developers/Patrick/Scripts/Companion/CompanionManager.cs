using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerObject;
    [SerializeField]
    private ObjectPoolManager poolManager;
    [SerializeField]
    private CompanionBossData bossData;
    [SerializeField]
    private CompanionFriendData friendData;

    private Rigidbody2D rb;

    private CompanionBoss bossScript;
    private CompanionFriend friendScript;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        bossScript = gameObject.AddComponent<CompanionBoss>();
        bossScript.InitialiseComponent(ref bossData, ref rb, ref playerObject);

        friendScript = gameObject.AddComponent<CompanionFriend>();
    }

    // Update is called once per frame
    void Update()
    {
        bossScript.CompanionUpdate();
    }

    private void FixedUpdate()
    {
        bossScript.CompanionFixedUpdate();
    }
}
