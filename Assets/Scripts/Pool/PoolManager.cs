using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    [System.Serializable]
    public class Pool
    {
        public string key;
        public GameObject prefab;
        public int size;
        public bool expandable;
    }
    
    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDict;

    public void Awake()
    {
        if(Instance == null)
            Instance = this;
        else Destroy(gameObject);
        poolDict = new Dictionary<string, Queue<GameObject>>();
        foreach (var pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for(int i=0;i<pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDict.Add(pool.key,objectPool);
        }

    }
    public GameObject Spawn(string key, Vector3 position, Quaternion rotation)
    {
        if (!poolDict.ContainsKey(key))
        {
            Debug.LogWarning($"[PoolManager] 没有找到池：{key}");
            return null;
        }

        Queue<GameObject> queue = poolDict[key];

        GameObject obj = null;
        if (queue.Count > 0 && !queue.Peek().activeSelf)
        {
            obj = queue.Dequeue();
        }
        else
        {
            Pool poolConfig = pools.Find(p => p.key == key);
            if (poolConfig != null && poolConfig.expandable)
            {
                obj = Instantiate(poolConfig.prefab);
            }
            else
            {
                obj = queue.Dequeue(); // 如果不扩容就复用旧对象（可能还在用）
            }
        }

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        // 如果实现了 IPoolable，则调用 OnSpawn
        IPoolable poolable = obj.GetComponent<IPoolable>();
        poolable?.OnSpawn();

        // 仍需入队（保证回收时可用）
        queue.Enqueue(obj);

        return obj;
    }

    public void Despawn(GameObject obj)
    {
        obj.SetActive(false);

        // 如果实现了 IPoolable，则调用 OnDespawn
        IPoolable poolable = obj.GetComponent<IPoolable>();
        poolable?.OnDespawn();
    }
}
