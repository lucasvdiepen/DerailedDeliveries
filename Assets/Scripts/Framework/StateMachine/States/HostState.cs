using FishNet;
using System.Collections;

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

            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection("localhost");
        }
    }
}