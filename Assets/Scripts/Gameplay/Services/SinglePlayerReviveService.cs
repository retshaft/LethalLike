using LethalLike.Gameplay;
using UnityEngine;

namespace LethalLike.Gameplay.Services
{
    public class SinglePlayerReviveService : MonoBehaviour, IReviveService
    {
        [SerializeField] private bool allowImmediateRevive;

        public bool TryRevive(PlayerHealth playerHealth)
        {
            if (!allowImmediateRevive || playerHealth == null)
            {
                return false;
            }

            // TODO(coop): replace with co-op revive interaction flow
            playerHealth.RestoreToFull();
            return true;
        }
    }
}
