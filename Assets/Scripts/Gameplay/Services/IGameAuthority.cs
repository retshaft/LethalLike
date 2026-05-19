using LethalLike.Gameplay;

namespace LethalLike.Gameplay.Services
{
    public interface IGameAuthority
    {
        bool TryDeliverSeed(Seed seed, TeamScoreManager teamScoreManager);
        bool TryApplyDamage(PlayerHealth playerHealth, float damageAmount);
    }
}
