using FishNet;
using System.Collections;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the join menu.
    /// </summary>
    public class JoinState : MenuState
    {
        public override IEnumerator OnStateEnter()
        {
            yield return base.OnStateEnter();

            InstanceFinder.ClientManager.StartConnection();
        }
    }
}