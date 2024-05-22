using FishNet;
using System.Collections;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    public class ScoreState : MenuState
    {
        public override IEnumerator OnStateEnter()
        {
            yield return base.OnStateEnter();

            InstanceFinder.ClientManager.StopConnection();
        }
    }
}