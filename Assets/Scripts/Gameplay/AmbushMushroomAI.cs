using System.Collections;
using LethalLike.Gameplay.Services;
using UnityEngine;

namespace LethalLike.Gameplay
{
    public class AmbushMushroomAI : MonoBehaviour
    {
        private enum EnemyState
        {
            Idle,
            Chase,
            Attack,
            Cooldown
        }

        [Header("Ambush")]
        [SerializeField] private float ambushDetectRange = 7f;
        [SerializeField] private float fieldOfView = 130f;
        [SerializeField] private float revealDuration = 0.15f;
        [SerializeField] private float lungeDuration = 0.25f;
        [SerializeField] private float lungeDistance = 4f;
        [SerializeField] private float attackRadius = 1.6f;
        [SerializeField] private float attackDamage = 18f;
        [SerializeField] private float attackCooldown = 3f;
        [SerializeField] private LayerMask attackMask = ~0;

        [Header("References")]
        [SerializeField] private MonoBehaviour targetProvider;
        [SerializeField] private MonoBehaviour authorityProvider;
        [SerializeField] private Transform visualRoot;

        [Header("Visual")]
        [SerializeField] private Vector3 hiddenScale = new Vector3(0.65f, 0.65f, 0.65f);
        [SerializeField] private Vector3 exposedScale = Vector3.one;

        private IEnemyTargetProvider _targetProvider;
        private IGameAuthority _authority;
        private EnemyState _state = EnemyState.Idle;
        private float _nextAttackTime;
        private Coroutine _attackRoutine;

        private void Awake()
        {
            _targetProvider = targetProvider as IEnemyTargetProvider;
            _authority = authorityProvider as IGameAuthority;
            SetVisualScale(hiddenScale);
        }

        private void Update()
        {
            if (_state == EnemyState.Attack)
            {
                return;
            }

            if (Time.time < _nextAttackTime)
            {
                SetState(EnemyState.Cooldown);
                return;
            }

            if (_targetProvider == null || !_targetProvider.TryGetTarget(out PlayerHealth target))
            {
                SetState(EnemyState.Idle);
                SetVisualScale(hiddenScale);
                return;
            }

            if (!CanAmbush(target))
            {
                SetState(EnemyState.Idle);
                SetVisualScale(hiddenScale);
                return;
            }

            if (_attackRoutine == null)
            {
                _attackRoutine = StartCoroutine(AmbushAttackRoutine(target));
            }
        }

        private bool CanAmbush(PlayerHealth target)
        {
            Vector3 toTarget = target.transform.position - transform.position;
            float distance = toTarget.magnitude;
            if (distance > ambushDetectRange)
            {
                return false;
            }

            Vector3 direction = toTarget.normalized;
            float angleToTarget = Vector3.Angle(transform.forward, direction);
            return angleToTarget <= fieldOfView * 0.5f;
        }

        private IEnumerator AmbushAttackRoutine(PlayerHealth target)
        {
            SetState(EnemyState.Chase);
            float safeRevealDuration = Mathf.Max(0f, revealDuration);
            float revealStartTime = Time.time;
            while (safeRevealDuration > 0f && Time.time - revealStartTime < safeRevealDuration)
            {
                float t = Mathf.Clamp01((Time.time - revealStartTime) / safeRevealDuration);
                SetVisualScale(Vector3.Lerp(hiddenScale, exposedScale, t));
                yield return null;
            }

            SetVisualScale(exposedScale);
            SetState(EnemyState.Attack);

            Vector3 startPosition = transform.position;
            Vector3 destination = startPosition + transform.forward * lungeDistance;
            if (target != null)
            {
                destination = target.transform.position;
            }

            float safeLungeDuration = Mathf.Max(0f, lungeDuration);
            float lungeStartTime = Time.time;
            while (safeLungeDuration > 0f && Time.time - lungeStartTime < safeLungeDuration)
            {
                float t = Mathf.Clamp01((Time.time - lungeStartTime) / safeLungeDuration);
                transform.position = Vector3.Lerp(startPosition, destination, t);
                yield return null;
            }

            transform.position = destination;
            TryAttack(target);
            _nextAttackTime = Time.time + attackCooldown;
            SetState(EnemyState.Cooldown);

            _attackRoutine = null;
        }

        private void TryAttack(PlayerHealth intendedTarget)
        {
            if (_authority == null)
            {
                return;
            }

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius, attackMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (!hitColliders[i].TryGetComponent(out PlayerHealth hitPlayer))
                {
                    hitPlayer = hitColliders[i].GetComponentInParent<PlayerHealth>();
                }

                if (hitPlayer == null || hitPlayer != intendedTarget)
                {
                    continue;
                }

                _authority.TryApplyDamage(hitPlayer, attackDamage);
                return;
            }
        }

        private void SetVisualScale(Vector3 scale)
        {
            Transform root = visualRoot != null ? visualRoot : transform;
            root.localScale = scale;
        }

        private void SetState(EnemyState state)
        {
            _state = state;
        }
    }
}
