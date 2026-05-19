using LethalLike.Gameplay;
using UnityEngine;

namespace LethalLike.Gameplay.Services
{
    public class LocalAuthority : MonoBehaviour, IGameAuthority
    {
        public bool TryDeliverSeed(Seed seed, TeamScoreManager teamScoreManager)
        {
            if (seed == null || teamScoreManager == null || seed.IsDelivered)
            {
                return false;
            }

            // TODO(coop): move this into NetworkAuthority / server-authoritative flow
            teamScoreManager.AddScore(seed.Value, seed.SeedId);
            seed.MarkDelivered();
            return true;
        }

        public bool TryApplyDamage(PlayerHealth playerHealth, float damageAmount)
        {
            if (playerHealth == null || damageAmount <= 0f)
            {
                return false;
            }

            // TODO(coop): move this into NetworkAuthority / server-authoritative flow
            playerHealth.TakeDamage(damageAmount);
            return true;
        }
    }
}
