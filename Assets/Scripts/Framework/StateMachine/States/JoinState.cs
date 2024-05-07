using FishNet;
using FishNet.Discovery;
using System.Collections;
using System.Net;
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

            yield return base.OnStateExit();
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