using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objectTypes;
    private Dictionary<string, int> lookupTable;
    [SerializeField]
    private List<ObjectPool<GameObject>> objectPools;
    // Start is called before the first frame update
    void Start()
    {
        lookupTable = new Dictionary<string, int>();
        objectPools = new List<ObjectPool<GameObject>>();

        for (int i = 0; i < objectTypes.Count; i++)
        {
            lookupTable[objectTypes[i].name] = i;
            GameObject objType = objectTypes[i];
            objectPools.Add(new ObjectPool<GameObject>(
                createFunc: () => Instantiate(objType),
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: 5,
                maxSize: 500
                ));
        }
    }

    public GameObject GetFreeObject(string objectTypeName)
    {
        return objectPools[lookupTable[objectTypeName]].Get();
    }

    public void ReleaseObject(string objectTypeName, GameObject releaseObject)
    {
        //Debug.Log("Release " + objectTypeName);
        objectPools[lookupTable[objectTypeName]].Release(releaseObject);
    }
}
