using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private GameObject prefab;
    private int maxSize;

    private readonly Queue<GameObject> available = new Queue<GameObject>();
    private readonly HashSet<GameObject> active = new HashSet<GameObject>();

    public int ActiveCount => active.Count;

    public void Initialize(GameObject sourcePrefab, int prewarmCount, int limit, Transform parent = null)
    {
        prefab = sourcePrefab;
        maxSize = Mathf.Max(1, limit);

        if (parent != null)
        {
            transform.SetParent(parent, false);
        }

        for (int i = 0; i < prewarmCount; i++)
        {
            GameObject instance = CreateInstance();
            if (instance == null)
            {
                break;
            }

            Return(instance);
        }
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject instance = null;

        if (available.Count > 0)
        {
            instance = available.Dequeue();
        }
        else if (active.Count < maxSize)
        {
            instance = CreateInstance();
        }

        if (instance == null)
        {
            return null;
        }

        active.Add(instance);
        instance.transform.SetPositionAndRotation(position, rotation);
        instance.SetActive(true);
        return instance;
    }

    public void Return(GameObject instance)
    {
        if (instance == null)
        {
            return;
        }

        if (active.Contains(instance))
        {
            active.Remove(instance);
        }

        if (!available.Contains(instance))
        {
            instance.transform.SetParent(transform, false);
            instance.SetActive(false);
            available.Enqueue(instance);
        }
    }

    private GameObject CreateInstance()
    {
        if (prefab == null)
        {
            return null;
        }

        GameObject instance = Instantiate(prefab, transform);
        PooledObject pooled = instance.GetComponent<PooledObject>();
        if (pooled == null)
        {
            pooled = instance.AddComponent<PooledObject>();
        }

        pooled.ownerPool = this;
        return instance;
    }
}
