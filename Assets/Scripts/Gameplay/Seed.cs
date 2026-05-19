using System;
using UnityEngine;

namespace LethalLike.Gameplay
{
    public class Seed : MonoBehaviour
    {
        [SerializeField] private string seedId = "seed_basic";
        [SerializeField] private string seedName = "Basic Seed";
        [SerializeField] private int value = 10;
        [SerializeField] private int dangerValue = 10;

        [Header("Audio Hooks")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pickupClip;
        [SerializeField] private AudioClip dropClip;

        public static event Action<Seed> OnSeedPickedUp;
        public static event Action<Seed> OnSeedDropped;
        public static event Action<Seed> OnSeedDelivered;

        public string SeedId => seedId;
        public string SeedName => seedName;
        public int Value => value;
        public int DangerValue => Mathf.Max(0, dangerValue);
        public bool IsDelivered { get; private set; }

        public void OnPickedUp()
        {
            PlayClip(pickupClip);
            OnSeedPickedUp?.Invoke(this);
        }

        public void OnDropped()
        {
            PlayClip(dropClip);
            OnSeedDropped?.Invoke(this);
        }

        public void MarkDelivered()
        {
            if (IsDelivered)
            {
                return;
            }

            IsDelivered = true;
            OnSeedDelivered?.Invoke(this);
        }

        private void PlayClip(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
