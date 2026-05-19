using LethalLike.Gameplay.Services;
using UnityEngine;
using UnityEngine.AI;

namespace LethalLike.Gameplay
{
    public class PlantEnemyAI : MonoBehaviour
    {
        [SerializeField] private float detectRange = 12f;
        [SerializeField] private float attackRange = 1.8f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private MonoBehaviour authorityProvider;

        [Header("Audio Hooks")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip detectedClip;
        [SerializeField] private AudioClip attackClip;
        [SerializeField] private AudioClip diedClip;

        private IGameAuthority _authority;
        private NavMeshAgent _agent;
        private PlayerHealth _target;
        private float _lastAttackTime;
        private bool _detectedPlayed;

        private void Awake()
        {
            _authority = authorityProvider as IGameAuthority;
            _agent = GetComponent<NavMeshAgent>();

            if (_agent != null)
            {
                _agent.speed = moveSpeed;
            }
        }

        private void Update()
        {
            if (_target == null || _target.IsDead)
            {
                _target = FindObjectOfType<PlayerHealth>();
                _detectedPlayed = false;
            }

            if (_target == null)
            {
                return;
            }

            Vector3 targetPosition = _target.transform.position;
            float distance = Vector3.Distance(transform.position, targetPosition);

            if (distance > detectRange)
            {
                StopMove();
                return;
            }

            if (!_detectedPlayed)
            {
                _detectedPlayed = true;
                PlayClip(detectedClip);
            }

            if (distance > attackRange)
            {
                MoveTo(targetPosition);
                return;
            }

            StopMove();
            TryAttack();
        }

        public void NotifyKilled()
        {
            PlayClip(diedClip);
        }

        private void TryAttack()
        {
            if (_authority == null)
            {
                return;
            }

            if (Time.time - _lastAttackTime < attackCooldown)
            {
                return;
            }

            _lastAttackTime = Time.time;
            PlayClip(attackClip);
            _authority.TryApplyDamage(_target, attackDamage);
        }

        private void MoveTo(Vector3 targetPosition)
        {
            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.SetDestination(targetPosition);
                return;
            }

            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * (moveSpeed * Time.deltaTime);
        }

        private void StopMove()
        {
            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.ResetPath();
            }
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
