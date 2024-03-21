using DerailedDeliveries.Framework.InputParser;
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

            PlayerManager.Instance.IsSpawnEnabled = true;
        }

        public override IEnumerator OnStateExit()
        {
            yield return base.OnStateExit();

            PlayerManager.Instance.IsSpawnEnabled = false;
        }
    }
}