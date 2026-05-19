using System.Collections.Generic;
using LethalLike.Gameplay.Services;
using UnityEngine;

namespace LethalLike.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class SporeCloud : MonoBehaviour
    {
        [SerializeField] private float lifetime = 8f;
        [SerializeField] private float tickInterval = 1f;
        [SerializeField] private float tickDamage = 4f;
        [SerializeField] private MonoBehaviour authorityProvider;
        [SerializeField] private ParticleSystem cloudVfx;
        [SerializeField] private GameObject fogVisual;

        private readonly HashSet<PlayerHealth> _targetsInside = new();
        private IGameAuthority _authority;
        private float _nextTickTime;

        private void Awake()
        {
            _authority = authorityProvider as IGameAuthority;
            Collider triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
        }

        private void OnEnable()
        {
            _nextTickTime = Time.time + tickInterval;
            if (cloudVfx != null)
            {
                cloudVfx.Play();
            }

            if (fogVisual != null)
            {
                fogVisual.SetActive(true);
            }

            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (_authority == null || Time.time < _nextTickTime)
            {
                return;
            }

            _nextTickTime = Time.time + tickInterval;
            foreach (PlayerHealth target in _targetsInside)
            {
                if (target == null || target.IsDead)
                {
                    continue;
                }

                _authority.TryApplyDamage(target, tickDamage);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerHealth target = GetTarget(other);
            if (target != null)
            {
                _targetsInside.Add(target);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerHealth target = GetTarget(other);
            if (target != null)
            {
                _targetsInside.Remove(target);
            }
        }

        private PlayerHealth GetTarget(Collider other)
        {
            if (other.TryGetComponent(out PlayerHealth target))
            {
                return target;
            }

            return other.GetComponentInParent<PlayerHealth>();
        }
    }
}
