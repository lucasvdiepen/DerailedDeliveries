using DerailedDeliveries.Framework.Gameplay;

namespace DerailedDeliveries.Framework.UI.TextUpdaters.Score
{
    /// <summary>
    /// A <see cref="TextUpdater"/> responsible for updating the text of the delivered correctly score.
    /// </summary>
    public class DeliveredIncorrectlyTextUpdater : TextUpdater
    {
        private void OnEnable() => ReplaceTag(LevelTracker.Instance.IncorrectScore.ToString());
    }
}