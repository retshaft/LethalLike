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
            SpawnAdditionalEnemies(spawnCount, limitToSpawnPointCount: true);
        }

        public void SpawnAdditionalEnemies(int count)
        {
            SpawnAdditionalEnemies(count, limitToSpawnPointCount: false);
        }

        private void SpawnAdditionalEnemies(int count, bool limitToSpawnPointCount)
        {
            if (enemyPrefab == null || spawnPoints.Count == 0 || count <= 0)
            {
                return;
            }

            int targetCount = limitToSpawnPointCount ? Mathf.Min(count, spawnPoints.Count) : count;
            for (int i = 0; i < targetCount; i++)
            {
                Transform spawnPoint = limitToSpawnPointCount ? spawnPoints[i] : spawnPoints[Random.Range(0, spawnPoints.Count)];
                // TODO(coop): move this into NetworkAuthority / server-authoritative flow
                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }
}
