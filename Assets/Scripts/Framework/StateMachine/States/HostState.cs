using DerailedDeliveries.Framework.InputParser;
using FishNet;
using FishNet.Transporting;
using System.Collections;
using UnityEngine;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the host menu.
    /// </summary>
    public class HostState : MenuState
    {
        public override IEnumerator OnStateEnter()
        {
            yield return base.OnStateEnter();

            InstanceFinder.ClientManager.OnClientConnectionState += OnClientStateChanged;

            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection("localhost");
        }

        public override IEnumerator OnStateExit()
        {
            yield return base.OnStateExit();

            PlayerManager.Instance.IsSpawnEnabled = false;
        }

        private void OnClientStateChanged(ClientConnectionStateArgs args)
        {
            if(args.ConnectionState != LocalConnectionState.Started)
                return;

            PlayerManager.Instance.IsSpawnEnabled = true;
        }
    }
}