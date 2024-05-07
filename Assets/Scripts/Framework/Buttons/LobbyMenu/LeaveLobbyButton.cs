using FishNet;

using DerailedDeliveries.Framework.StateMachine.States;

namespace DerailedDeliveries.Framework.Buttons.LobbyMenu
{
    public class LeaveLobbyButton : BasicButton
    {
        private protected override void OnButtonPressed()
        {
            if(InstanceFinder.NetworkManager.IsOffline)
            {
                StateMachine.StateMachine.Instance.GoToState<MainMenuState>();
                return;
            }

            InstanceFinder.ServerManager.StopConnection(true);
            InstanceFinder.ClientManager.StopConnection();
        }
    }
}