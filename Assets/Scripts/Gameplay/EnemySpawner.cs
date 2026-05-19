using System.Collections.Generic;
using UnityEngine;

namespace LethalLike.Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private PlantEnemyAI enemyPrefab;
        [SerializeField] private List<Transform> spawnPoints = new();
        [SerializeField] private int spawnCount = 1;

        private void Start()
        {
            if (enemyPrefab == null || spawnPoints.Count == 0)
            {
                return;
            }

            int count = Mathf.Min(spawnCount, spawnPoints.Count);
            for (int i = 0; i < count; i++)
            {
                // TODO(coop): move this into NetworkAuthority / server-authoritative flow
                Instantiate(enemyPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            }
        }
    }
}
