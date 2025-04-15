using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] // List of game objects to create pools for
    private List<GameObject> _objectTypes;

    // Dictionary for the index of the prefab's object pool found by name
    private Dictionary<string, int> _lookupTable;
    // List of all the object pools
    private List<ObjectPool<GameObject>> _objectPools;


    // Start is called before the first frame update
    void Start()
    {
        _lookupTable = new Dictionary<string, int>();
        _objectPools = new List<ObjectPool<GameObject>>();

        // Creates an object pool for each type of game object and sets up it's index in the lookup table
        for (int i = 0; i < _objectTypes.Count; i++)
        {
            _lookupTable[_objectTypes[i].name] = i;
            GameObject objType = _objectTypes[i];
            _objectPools.Add(new ObjectPool<GameObject>(
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

    // Gets the next free object in the specified object pool
    public GameObject GetFreeObject(string objectTypeName)
    {
        return _objectPools[_lookupTable[objectTypeName]].Get();
    }

    // Releases the game object from its object pool
    public void ReleaseObject(string objectTypeName, GameObject releaseObject)
    {
        _objectPools[_lookupTable[objectTypeName]].Release(releaseObject);
    }
}
