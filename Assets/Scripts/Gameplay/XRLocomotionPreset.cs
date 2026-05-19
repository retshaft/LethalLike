using UnityEngine;

namespace LethalLike.Gameplay
{
    public class XRLocomotionPreset : MonoBehaviour
    {
        public enum LocomotionMode
        {
            Teleport,
            Smooth
        }

        [SerializeField] private LocomotionMode locomotionMode = LocomotionMode.Teleport;
        [SerializeField] private MonoBehaviour teleportProvider;
        [SerializeField] private MonoBehaviour continuousMoveProvider;
        [SerializeField] private MonoBehaviour continuousTurnProvider;
        [SerializeField] private MonoBehaviour snapTurnProvider;

        private void Awake()
        {
            ApplyPreset();
        }

        public void ApplyPreset()
        {
            bool teleport = locomotionMode == LocomotionMode.Teleport;

            if (teleportProvider != null)
            {
                teleportProvider.enabled = teleport;
            }

            if (snapTurnProvider != null)
            {
                snapTurnProvider.enabled = teleport;
            }

            if (continuousMoveProvider != null)
            {
                continuousMoveProvider.enabled = !teleport;
            }

            if (continuousTurnProvider != null)
            {
                continuousTurnProvider.enabled = !teleport;
            }
        }
    }
}
