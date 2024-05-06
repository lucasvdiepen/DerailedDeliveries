using FishNet;
using FishNet.Transporting;
using System.Collections;

using DerailedDeliveries.Framework.PlayerManagement;
using DerailedDeliveries.Framework.StateMachine.Attributes;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the host menu.
    /// </summary>
    [ParentState(typeof(LobbyState))]
    public class HostState : MenuState
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateEnter()
        {
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnnectionStateChanged;
            
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection("localhost");

            yield return base.OnStateEnter();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateExit()
        {
            InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnnectionStateChanged;

            if(InstanceFinder.NetworkManager.IsClient)
                PlayerManager.Instance.IsSpawnEnabled = false;

            yield return base.OnStateExit();
        }

        private void OnClientConnnectionStateChanged(ClientConnectionStateArgs args)
        {
            if (args.ConnectionState != LocalConnectionState.Started)
                return;

            StateMachine.Instance.GoToState<GameState>();
        }
    }
}