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
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection("localhost");

            yield return base.OnStateEnter();
        }

        public override IEnumerator OnStateExit()
        {
            if(InstanceFinder.NetworkManager.IsClient)
                PlayerManager.Instance.IsSpawnEnabled = false;

            yield return base.OnStateExit();
        }
    }
}