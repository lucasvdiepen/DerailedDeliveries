using FishNet;
using System.Collections;

using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the join menu.
    /// </summary>
    public class JoinState : MenuState
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateEnter()
        {
            InstanceFinder.ClientManager.StartConnection();

            yield return base.OnStateEnter();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateExit()
        {
            if (InstanceFinder.NetworkManager.IsClient)
                PlayerManager.Instance.IsSpawnEnabled = false;

            yield return base.OnStateExit();
        }
    }
}