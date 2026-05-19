using LethalLike.Gameplay.Services;
using UnityEngine;
using UnityEngine.AI;

namespace LethalLike.Gameplay
{
    public class SporeEnemyAI : MonoBehaviour
    {
        private enum EnemyState
        {
            Idle,
            Chase,
            Attack,
            Cooldown
        }

        [Header("Combat")]
        [SerializeField] private float detectRange = 14f;
        [SerializeField] private float detectRangePerThreat = 0.2f;
        [SerializeField] private float cloudDeployRange = 8f;
        [SerializeField] private float deployCooldown = 6f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2.2f;

        [Header("References")]
        [SerializeField] private MonoBehaviour targetProvider;
        [SerializeField] private MonoBehaviour authorityProvider;
        [SerializeField] private SporeCloud sporeCloudPrefab;
        [SerializeField] private Transform cloudSpawnPoint;
        [SerializeField] private ThreatManager threatManager;

        [Header("Audio Hooks")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip castClip;

        private IEnemyTargetProvider _targetProvider;
        private IGameAuthority _authority;
        private NavMeshAgent _agent;
        private EnemyState _state = EnemyState.Idle;
        private float _nextDeployTime;

        private void Awake()
        {
            _targetProvider = targetProvider as IEnemyTargetProvider;
            _authority = authorityProvider as IGameAuthority;
            _agent = GetComponent<NavMeshAgent>();
            if (_agent != null)
            {
                _agent.speed = moveSpeed;
            }
        }

        private void Update()
        {
            if (_targetProvider == null || !_targetProvider.TryGetTarget(out PlayerHealth target))
            {
                SetState(EnemyState.Idle);
                StopMove();
                return;
            }

            float effectiveDetectRange = detectRange + (threatManager != null ? threatManager.CurrentThreat * detectRangePerThreat : 0f);
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > effectiveDetectRange)
            {
                SetState(EnemyState.Idle);
                StopMove();
                return;
            }

            if (distance <= cloudDeployRange && Time.time >= _nextDeployTime)
            {
                SetState(EnemyState.Attack);
                DeployCloud();
                _nextDeployTime = Time.time + deployCooldown;
                SetState(EnemyState.Cooldown);
                return;
            }

            SetState(EnemyState.Chase);
            MoveTo(target.transform.position);
        }

        private void DeployCloud()
        {
            if (sporeCloudPrefab == null)
            {
                return;
            }

            Vector3 spawnPosition = cloudSpawnPoint != null ? cloudSpawnPoint.position : transform.position;
            Quaternion spawnRotation = cloudSpawnPoint != null ? cloudSpawnPoint.rotation : transform.rotation;
            SporeCloud cloud = Instantiate(sporeCloudPrefab, spawnPosition, spawnRotation);
            cloud.gameObject.SetActive(true);
            PlayClip(castClip);
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

        private void SetState(EnemyState nextState)
        {
            _state = nextState;
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
