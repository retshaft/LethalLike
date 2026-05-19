using System;
using UnityEngine;

namespace LethalLike.Gameplay
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;

        [Header("Audio Hooks")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip damagedClip;
        [SerializeField] private AudioClip diedClip;

        public float CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;

        public event Action<float, float> OnDamaged;
        public event Action OnDied;

        private void Awake()
        {
            RestoreToFull();
        }

        public void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            OnDamaged?.Invoke(CurrentHealth, maxHealth);
            PlayClip(damagedClip);

            if (IsDead)
            {
                OnDied?.Invoke();
                PlayClip(diedClip);
            }
        }

        public void RestoreToFull()
        {
            CurrentHealth = maxHealth;
            OnDamaged?.Invoke(CurrentHealth, maxHealth);
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
