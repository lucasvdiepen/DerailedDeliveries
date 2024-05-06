using System.Net;
using FishNet;
using FishNet.Discovery;
using FishNet.Transporting;
using System.Collections;
using UnityEngine;

using DerailedDeliveries.Framework.StateMachine.Attributes;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the join menu.
    /// </summary>
    [ParentState(typeof(LobbyState))]
    public class JoinState : MenuState
    {
        [SerializeField]
        private NetworkDiscovery _networkDiscovery;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateEnter()
        {
            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnnectionStateChanged;

            _networkDiscovery.ServerFoundCallback += OnServerFound;
            _networkDiscovery.SearchForServers();

            yield return base.OnStateEnter();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateExit()
        {
            StopSearchingServer();

            InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnnectionStateChanged;

            yield return base.OnStateExit();
        }

        private void OnClientConnnectionStateChanged(ClientConnectionStateArgs args)
        {
            if (args.ConnectionState != LocalConnectionState.Started)
                return;

            //StateMachine.Instance.GoToState<GameState>();
        }

        private void OnServerFound(IPEndPoint ipEndPoint)
        {
            InstanceFinder.ClientManager.StartConnection(ipEndPoint.Address.ToString());

            StopSearchingServer();
        }

        private void StopSearchingServer()
        {
            if (!_networkDiscovery.IsSearching)
                return;

            _networkDiscovery.StopSearchingOrAdvertising();
            _networkDiscovery.ServerFoundCallback -= OnServerFound;
        }
    }
}