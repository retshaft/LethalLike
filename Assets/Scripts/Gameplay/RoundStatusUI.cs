using TMPro;
using UnityEngine;

namespace LethalLike.Gameplay
{
    public class RoundStatusUI : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private TeamScoreManager teamScoreManager;
        [SerializeField] private TMP_Text statusText;

        private void OnEnable()
        {
            if (roundManager != null)
            {
                roundManager.OnRoundStateChanged += HandleRoundChanged;
                roundManager.OnTimeUpdated += HandleTimeUpdated;
                roundManager.OnRoundEnded += HandleRoundEnded;
            }

            if (teamScoreManager != null)
            {
                teamScoreManager.OnScoreChanged += HandleScoreChanged;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (roundManager != null)
            {
                roundManager.OnRoundStateChanged -= HandleRoundChanged;
                roundManager.OnTimeUpdated -= HandleTimeUpdated;
                roundManager.OnRoundEnded -= HandleRoundEnded;
            }

            if (teamScoreManager != null)
            {
                teamScoreManager.OnScoreChanged -= HandleScoreChanged;
            }
        }

        private void HandleRoundChanged(RoundManager.RoundState _) => Refresh();
        private void HandleTimeUpdated(float _) => Refresh();
        private void HandleRoundEnded(string _) => Refresh();
        private void HandleScoreChanged(int _, int __) => Refresh();

        private void Refresh()
        {
            if (statusText == null || roundManager == null)
            {
                return;
            }

            statusText.text = roundManager.GetStatusText();
        }
    }
}
