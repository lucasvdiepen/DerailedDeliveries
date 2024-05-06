using FishNet;
using FishNet.Discovery;
using FishNet.Transporting;
using System.Collections;
using UnityEngine;

using DerailedDeliveries.Framework.StateMachine.Attributes;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the host menu.
    /// </summary>
    [ParentState(typeof(LobbyState))]
    public class HostState : MenuState
    {
        [SerializeField]
        private NetworkDiscovery _networkDiscovery;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateEnter()
        {
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnnectionStateChanged;
            
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection("localhost");
            _networkDiscovery.AdvertiseServer();

            yield return base.OnStateEnter();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateExit()
        {
            _networkDiscovery.StopSearchingOrAdvertising();

            InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnnectionStateChanged;

            yield return base.OnStateExit();
        }

        private void OnClientConnnectionStateChanged(ClientConnectionStateArgs args)
        {
            if (args.ConnectionState != LocalConnectionState.Started)
                return;

            //StateMachine.Instance.GoToState<GameState>();
        }
    }
}