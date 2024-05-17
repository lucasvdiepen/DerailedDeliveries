using UnityEngine;

using DerailedDeliveries.Framework.Gameplay;

namespace DerailedDeliveries.Framework.UI.TextUpdaters.Score
{
    /// <summary>
    /// A <see cref="TextUpdater"/> responsible for updating the text of the current score.
    /// </summary>
    public class ScoreTextUpdater : TextUpdater
    {
        private void OnEnable() => ReplaceTag(LevelTracker.Instance.CurrentScore.ToString());
    }
}