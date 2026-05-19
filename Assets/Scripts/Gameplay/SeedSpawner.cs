using System.Collections.Generic;
using UnityEngine;

namespace LethalLike.Gameplay
{
    public class SeedSpawner : MonoBehaviour
    {
        [SerializeField] private Seed seedPrefab;
        [SerializeField] private List<Transform> spawnPoints = new();
        [SerializeField] private int spawnCount = 5;

        private void Start()
        {
            if (seedPrefab == null || spawnPoints.Count == 0)
            {
                return;
            }

            int count = Mathf.Min(spawnCount, spawnPoints.Count);
            for (int i = 0; i < count; i++)
            {
                // TODO(coop): move this into NetworkAuthority / server-authoritative flow
                Instantiate(seedPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            }
        }
    }
}
