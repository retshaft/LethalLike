using System;
using System.Collections.Generic;
using UnityEngine;

namespace LethalLike.Gameplay
{
    public class ThreatManager : MonoBehaviour
    {
        [Header("Threat")]
        [SerializeField] private float spawnPressureChancePerThreat = 0.03f;
        [SerializeField] private int pressureSpawnCount = 1;

        [Header("References")]
        [SerializeField] private EnemySpawner enemySpawner;

        [Header("Audio Hooks")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip threatRiseClip;

        private readonly HashSet<Seed> _carriedSeeds = new();

        public int CurrentThreat { get; private set; }
        public int CurrentCarriedSeedValue { get; private set; }

        public event Action<int, int> OnThreatChanged;

        private void OnEnable()
        {
            Seed.OnSeedPickedUp += HandleSeedPickedUp;
            Seed.OnSeedDropped += HandleSeedDropped;
            Seed.OnSeedDelivered += HandleSeedDelivered;
        }

        private void OnDisable()
        {
            Seed.OnSeedPickedUp -= HandleSeedPickedUp;
            Seed.OnSeedDropped -= HandleSeedDropped;
            Seed.OnSeedDelivered -= HandleSeedDelivered;
        }

        private void HandleSeedPickedUp(Seed seed)
        {
            if (seed == null || seed.IsDelivered)
            {
                return;
            }

            bool added = _carriedSeeds.Add(seed);
            if (!added)
            {
                return;
            }

            RefreshThreatAndValue(playAudio: true);
            TrySpawnPressureEnemy();
        }

        private void HandleSeedDropped(Seed seed)
        {
            if (seed == null)
            {
                return;
            }

            if (_carriedSeeds.Remove(seed))
            {
                RefreshThreatAndValue(playAudio: false);
            }
        }

        private void HandleSeedDelivered(Seed seed)
        {
            HandleSeedDropped(seed);
        }

        private void RefreshThreatAndValue(bool playAudio)
        {
            int nextThreat = 0;
            int nextValue = 0;
            foreach (Seed carriedSeed in _carriedSeeds)
            {
                if (carriedSeed == null || carriedSeed.IsDelivered)
                {
                    continue;
                }

                nextThreat += carriedSeed.DangerValue;
                nextValue += carriedSeed.Value;
            }

            bool increased = nextThreat > CurrentThreat;
            CurrentThreat = nextThreat;
            CurrentCarriedSeedValue = nextValue;
            OnThreatChanged?.Invoke(CurrentThreat, CurrentCarriedSeedValue);

            if (playAudio && increased && audioSource != null && threatRiseClip != null)
            {
                audioSource.PlayOneShot(threatRiseClip);
            }
        }

        private void TrySpawnPressureEnemy()
        {
            if (enemySpawner == null || CurrentThreat <= 0)
            {
                return;
            }

            float chance = Mathf.Clamp01(CurrentThreat * spawnPressureChancePerThreat);
            if (UnityEngine.Random.value <= chance)
            {
                enemySpawner.SpawnAdditionalEnemies(pressureSpawnCount);
            }
        }
    }
}
