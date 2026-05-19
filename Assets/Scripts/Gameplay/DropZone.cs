using LethalLike.Gameplay.Services;
using UnityEngine;

namespace LethalLike.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class DropZone : MonoBehaviour
    {
        [SerializeField] private TeamScoreManager teamScoreManager;
        [SerializeField] private MonoBehaviour authorityProvider;

        private IGameAuthority _authority;

        private void Awake()
        {
            _authority = authorityProvider as IGameAuthority;

            Collider triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_authority == null)
            {
                return;
            }

            if (!other.TryGetComponent(out Seed seed))
            {
                return;
            }

            if (_authority.TryDeliverSeed(seed, teamScoreManager))
            {
                Destroy(seed.gameObject);
            }
        }
    }
}
