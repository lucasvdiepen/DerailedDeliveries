using UnityEngine;

using DerailedDeliveries.Framework.Gameplay;

namespace DerailedDeliveries.Framework.UI.TextUpdaters.Score
{
    /// <summary>
    /// A <see cref="TextUpdater"/> responsible for updating the text of the delivered correctly score.
    /// </summary>
    public class ScorePercentageTextUpdater : TextUpdater
    {
        private void OnEnable() => ReplaceTag(Mathf.RoundToInt(LevelTracker.Instance.ScorePercentage).ToString());
    }
}