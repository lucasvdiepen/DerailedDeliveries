using FishNet;

namespace DerailedDeliveries.Framework.Buttons.LobbyMenu
{
    public class LeaveLobbyButton : BasicButton
    {
        private protected override void OnButtonPressed()
        {
            InstanceFinder.ServerManager.StopConnection(true);
            InstanceFinder.ClientManager.StopConnection();
        }
    }
}