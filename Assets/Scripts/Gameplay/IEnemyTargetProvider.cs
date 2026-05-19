namespace LethalLike.Gameplay
{
    public interface IEnemyTargetProvider
    {
        bool TryGetTarget(out PlayerHealth target);
    }
}
