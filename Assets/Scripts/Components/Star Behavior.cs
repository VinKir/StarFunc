#nullable enable

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StarBehavior : MonoBehaviour
{
    private LevelManager? levelManager = null;

    private void Awake()
    {
        levelManager = FindFirstObjectByType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        Destroy(gameObject);

        if (levelManager == null)
        {
            return;
        }

        levelManager.CollectStar();
    }
}