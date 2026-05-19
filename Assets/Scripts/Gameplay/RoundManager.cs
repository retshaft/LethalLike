using System;
using LethalLike.Gameplay.Services;
using UnityEngine;

namespace LethalLike.Gameplay
{
    public class RoundManager : MonoBehaviour
    {
        public enum RoundState
        {
            Ready,
            Playing,
            Ended
        }

        [Header("Round")]
        [SerializeField] private float roundDurationSeconds = 180f;
        [SerializeField] private bool autoStartOnPlay = true;

        [Header("References")]
        [SerializeField] private TeamScoreManager teamScoreManager;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private MonoBehaviour reviveServiceProvider;
        [SerializeField] private ThreatManager threatManager;

        [Header("Audio Hooks")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip roundStartClip;
        [SerializeField] private AudioClip roundEndClip;

        private IReviveService _reviveService;
        private float _timeRemaining;
        private string _endReason = string.Empty;

        public RoundState CurrentState { get; private set; } = RoundState.Ready;
        public float TimeRemaining => Mathf.Max(0f, _timeRemaining);
        public string EndReason => _endReason;

        public event Action<RoundState> OnRoundStateChanged;
        public event Action<float> OnTimeUpdated;
        public event Action<string> OnRoundEnded;

        private void Awake()
        {
            _reviveService = reviveServiceProvider as IReviveService;
            _timeRemaining = roundDurationSeconds;
        }

        private void OnEnable()
        {
            if (playerHealth != null)
            {
                playerHealth.OnDied += HandlePlayerDied;
            }
        }

        private void Start()
        {
            if (autoStartOnPlay)
            {
                StartRound();
            }
        }

        private void OnDisable()
        {
            if (playerHealth != null)
            {
                playerHealth.OnDied -= HandlePlayerDied;
            }
        }

        private void Update()
        {
            if (CurrentState != RoundState.Playing)
            {
                return;
            }

            _timeRemaining -= Time.deltaTime;
            OnTimeUpdated?.Invoke(TimeRemaining);

            if (_timeRemaining <= 0f)
            {
                EndRound("Time out");
            }
        }

        public void StartRound()
        {
            _timeRemaining = roundDurationSeconds;
            _endReason = string.Empty;

            if (teamScoreManager != null)
            {
                teamScoreManager.ResetScore();
            }

            SetState(RoundState.Playing);
            PlayClip(roundStartClip);
        }

        public void EndRound(string reason)
        {
            if (CurrentState == RoundState.Ended)
            {
                return;
            }

            _endReason = reason;
            SetState(RoundState.Ended);
            OnRoundEnded?.Invoke(_endReason);
            PlayClip(roundEndClip);
        }

        public string GetStatusText()
        {
            int delivered = teamScoreManager != null ? teamScoreManager.DeliveredSeedCount : 0;
            int score = teamScoreManager != null ? teamScoreManager.Score : 0;
            int carriedValue = threatManager != null ? threatManager.CurrentCarriedSeedValue : 0;
            int currentThreat = threatManager != null ? threatManager.CurrentThreat : 0;
            return $"State: {CurrentState}\nTime: {TimeRemaining:F1}\nDelivered: {delivered}\nScore: {score}\nCarry Value: {carriedValue}\nThreat: {currentThreat}\nReason: {_endReason}";
        }

        private void HandlePlayerDied()
        {
            if (_reviveService != null && _reviveService.TryRevive(playerHealth))
            {
                return;
            }

            EndRound("Player down");
        }

        private void SetState(RoundState state)
        {
            CurrentState = state;
            OnRoundStateChanged?.Invoke(CurrentState);
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
