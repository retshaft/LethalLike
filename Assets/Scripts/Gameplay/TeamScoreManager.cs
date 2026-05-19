using System;
using UnityEngine;

namespace LethalLike.Gameplay
{
    public class TeamScoreManager : MonoBehaviour
    {
        [Header("Audio Hooks")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip seedDeliveredClip;

        public int Score { get; private set; }
        public int DeliveredSeedCount { get; private set; }

        public event Action<int, int> OnScoreChanged;

        public void AddScore(int value, string seedId)
        {
            Score += Mathf.Max(0, value);
            DeliveredSeedCount += 1;
            OnScoreChanged?.Invoke(Score, DeliveredSeedCount);

            if (audioSource != null && seedDeliveredClip != null)
            {
                audioSource.PlayOneShot(seedDeliveredClip);
            }

            Debug.Log($"Delivered seed [{seedId}] -> +{value} score. Total={Score}");
        }

        public void ResetScore()
        {
            Score = 0;
            DeliveredSeedCount = 0;
            OnScoreChanged?.Invoke(Score, DeliveredSeedCount);
        }
    }
}
