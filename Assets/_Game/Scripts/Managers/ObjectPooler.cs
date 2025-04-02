using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public enum PoolType
    {
        DestroyParticle
    }

    [System.Serializable]
    public class Pool
    {
        public PoolType PoolType;
        public GameObject Prefab;
        public int Size;
        public bool Expandable;
    }

    public List<Pool> Pools;
    public Dictionary<PoolType, List<GameObject>> PoolDictionary;

    void Awake()
    {
        PoolDictionary = new Dictionary<PoolType, List<GameObject>>();

        foreach (Pool pool in Pools)
        {
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject obj = Instantiate(pool.Prefab);
                obj.transform.SetParent(transform);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            PoolDictionary.Add(pool.PoolType, objectPool);
        }
    }

    public GameObject SpawnFromPool(PoolType poolType, Vector3 position, Quaternion rotation)
    {
        if (!PoolDictionary.ContainsKey(poolType))
        {
            Debug.LogWarning("Pool with type '" + poolType + "' doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = GetPooledObject(poolType);

        if (objectToSpawn == null)
        {
            Debug.LogWarning("No available objects in pool '" + poolType + "' and pool is not expandable.");
            return null;
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }

    private GameObject GetPooledObject(PoolType poolType)
    {
        List<GameObject> objects = PoolDictionary[poolType];

        foreach (GameObject obj in objects)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        Pool poolConfig = Pools.Find(x => x.PoolType == poolType);
        if (poolConfig != null && poolConfig.Expandable)
        {
            GameObject obj = Instantiate(poolConfig.Prefab);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            objects.Add(obj);
            return obj;
        }

        return null;
    }
}
