using FishNet;
using System.Collections;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the score screen.
    /// </summary>
    public class ScoreState : MenuState
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateEnter()
        {
            yield return base.OnStateEnter();

            InstanceFinder.ClientManager.StopConnection();
        }
    }
}