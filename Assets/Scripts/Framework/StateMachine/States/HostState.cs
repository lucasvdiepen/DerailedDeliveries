using FishNet;
using FishNet.Discovery;
using FishNet.Transporting;
using System.Collections;
using UnityEngine;

using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the host menu.
    /// </summary>
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
            // todo: we need to call StopSearchingOrAdvertising when we have finished the host screen.
            //_networkDiscovery.StopSearchingOrAdvertising();

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