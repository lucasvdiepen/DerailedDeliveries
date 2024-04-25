using DerailedDeliveries.Framework.Gameplay;

namespace DerailedDeliveries.Framework.UI.TextUpdaters
{
    /// <summary>
    /// A <see cref="TextUpdater"/> class that updates the current score text.
    /// </summary>
    public class ScoreTextUpdater : TextUpdater
    {
        private void OnEnable()
        {
            LevelTracker.Instance.OnScoreChanged += UpdateText;
            UpdateText(LevelTracker.Instance.CurrentScore);
        }

        private void OnDisable()
        {
            if(LevelTracker.Instance != null)
                LevelTracker.Instance.OnScoreChanged -= UpdateText;
        }

        private void UpdateText(int score) => ReplaceTag(score.ToString());
    }
}