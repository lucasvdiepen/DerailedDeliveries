using DerailedDeliveries.Framework.Gameplay;

namespace DerailedDeliveries.Framework.UI.TextUpdaters.Score
{
    /// <summary>
    /// A <see cref="TextUpdater"/> class that updates the current time score text.
    /// </summary>
    public class TimeScoreTextUpdater : TextUpdater
    {
        private void OnEnable() => ReplaceTag(LevelTracker.Instance.TimeScore.ToString());
    }
}