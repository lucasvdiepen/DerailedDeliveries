using UnityEngine;

using DerailedDeliveries.Framework.Gameplay;

namespace DerailedDeliveries.Framework.UI.TextUpdaters.Score
{
    /// <summary>
    /// A <see cref="TextUpdater"/> responsible for updating the text of the delivered correctly score.
    /// </summary>
    public class ScorePercentageTextUpdater : TextUpdater
    {
        private void OnEnable()
        {
            int scorePercentage = Mathf.Max(0, Mathf.RoundToInt(LevelTracker.Instance.ScorePercentage));
            ReplaceTag(scorePercentage + "%");
        }
    }
}