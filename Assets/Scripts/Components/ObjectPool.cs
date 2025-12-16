using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 10;

    private readonly List<GameObject> pool = new();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
            CreateNewObject();
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public GameObject Get()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeSelf)
                return obj;
        }

        return CreateNewObject();
    }

    public void Release(GameObject obj)
    {
        obj.SetActive(false);
    }
}
