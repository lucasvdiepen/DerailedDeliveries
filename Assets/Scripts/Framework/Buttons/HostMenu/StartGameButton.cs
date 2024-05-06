using DerailedDeliveries.Framework.GameManagement;

namespace DerailedDeliveries.Framework.Buttons.HostMenu
{
    /// <summary>
    /// A button responsible for starting the game.
    /// </summary>
    public class StartGameButton : BasicButton
    {
        private protected override void OnButtonPressed() => GameManager.Instance.StartGame();
    }
}