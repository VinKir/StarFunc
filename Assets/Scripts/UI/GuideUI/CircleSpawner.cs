using UnityEngine;
using System.Collections;

public class CircleSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool pool;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float lifeTime = 5f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOne()
    {
        GameObject obj = pool.Get();

        obj.transform.position = transform.position;
        obj.SetActive(true);

        StartCoroutine(LifeTimer(obj));
    }

    private IEnumerator LifeTimer(GameObject obj)
    {
        yield return new WaitForSeconds(lifeTime);

        if (obj.activeSelf)
            pool.Release(obj);
    }
}
