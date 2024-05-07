using DerailedDeliveries.Framework.GameManagement;
using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.Buttons.HostMenu
{
    /// <summary>
    /// A button responsible for starting the game.
    /// </summary>
    public class StartGameButton : BasicButton
    {
        private protected override void OnButtonPressed() => GameManager.Instance.StartGame();

        private protected override void OnEnable()
        {
            base.OnEnable();

            PlayerManager.Instance.OnPlayersUpdated += UpdateButtonInteractivity;
            UpdateButtonInteractivity();
        }

        private protected override void OnDisable()
        {
            base.OnDisable();

            if(PlayerManager.Instance == null)
                return;

            PlayerManager.Instance.OnPlayersUpdated -= UpdateButtonInteractivity;
        }

        private void UpdateButtonInteractivity() => Button.interactable = PlayerManager.Instance.PlayerCount > 0;
    }
}