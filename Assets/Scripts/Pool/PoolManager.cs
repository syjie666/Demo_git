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
            Debug.LogWarning($"[PoolManager] û���ҵ��أ�{key}");
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
                obj = queue.Dequeue(); // ��������ݾ͸��þɶ��󣨿��ܻ����ã�
            }
        }

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        // ���ʵ���� IPoolable������� OnSpawn
        IPoolable poolable = obj.GetComponent<IPoolable>();
        poolable?.OnSpawn();

        // ������ӣ���֤����ʱ���ã�
        queue.Enqueue(obj);

        return obj;
    }

    public void Despawn(GameObject obj)
    {
        obj.SetActive(false);

        // ���ʵ���� IPoolable������� OnDespawn
        IPoolable poolable = obj.GetComponent<IPoolable>();
        poolable?.OnDespawn();
    }
}
