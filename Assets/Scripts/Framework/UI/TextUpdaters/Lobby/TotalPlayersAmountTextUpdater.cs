using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.UI.TextUpdaters.Lobby
{
    /// <summary>
    /// A <see cref="TextUpdater"/> class which updates the total players amount text.
    /// </summary>
    public class TotalPlayersAmountTextUpdater : TextUpdater
    {
        private void OnEnable()
        {
            PlayerManager.Instance.OnPlayersUpdated += UpdateText;

            ReplaceTag("[maxPlayers]", PlayerManager.Instance.MaxPlayers.ToString());
            UpdateText();
        }

        private void OnDisable()
        {
            if(PlayerManager.Instance == null)
                return;

            PlayerManager.Instance.OnPlayersUpdated -= UpdateText;
        }

        private void UpdateText() => ReplaceTag("[players]", PlayerManager.Instance.PlayerCount.ToString());
    }
}