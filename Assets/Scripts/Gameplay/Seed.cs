using UnityEngine;

namespace LethalLike.Gameplay
{
    public class Seed : MonoBehaviour
    {
        [SerializeField] private string seedId = "seed_basic";
        [SerializeField] private string seedName = "Basic Seed";
        [SerializeField] private int value = 10;

        [Header("Audio Hooks")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pickupClip;
        [SerializeField] private AudioClip dropClip;

        public string SeedId => seedId;
        public string SeedName => seedName;
        public int Value => value;
        public bool IsDelivered { get; private set; }

        public void OnPickedUp()
        {
            PlayClip(pickupClip);
        }

        public void OnDropped()
        {
            PlayClip(dropClip);
        }

        public void MarkDelivered()
        {
            IsDelivered = true;
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
