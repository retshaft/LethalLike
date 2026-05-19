using LethalLike.Gameplay;

namespace LethalLike.Gameplay.Services
{
    public interface IReviveService
    {
        bool TryRevive(PlayerHealth playerHealth);
    }
}
