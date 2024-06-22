using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class PoolObjects<T> : IDisposable where T : MonoBehaviour
{
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly Queue<T> _pool;

    public PoolObjects(T prefab, Transform parent, int preloadCount)
    {
        _prefab = prefab;
        _parent = parent;
        _pool = new Queue<T>();

        for (var i = 0; i < preloadCount; i++)
        {
            CreateObject();
        }
    }

    public T GetFree(bool returnActive = false)
    {
        foreach (var instance in _pool)
        {
            if (instance == null)
            {
                continue;
            }

            if (instance.gameObject.activeSelf == false)
            {
                instance.gameObject.SetActive(returnActive);
                return instance;
            }
        }

        return CreateObject(returnActive);
    }

    private T CreateObject(bool returnActive = false)
    {
        var instance = Object.Instantiate(_prefab, _parent);
        instance.gameObject.SetActive(returnActive);
        _pool.Enqueue(instance);

        return instance;
    }
    
    public void Dispose()
    {
        _pool.Clear();
    }
}
