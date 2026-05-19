using UnityEngine;

namespace LethalLike.Gameplay
{
    public class FixedEnemyTargetProvider : MonoBehaviour, IEnemyTargetProvider
    {
        [SerializeField] private PlayerHealth target;

        public bool TryGetTarget(out PlayerHealth resolvedTarget)
        {
            resolvedTarget = target;
            return resolvedTarget != null && !resolvedTarget.IsDead;
        }
    }
}
