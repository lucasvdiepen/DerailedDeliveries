using DerailedDeliveries.Framework.InputParser;
using DG.Tweening;
using FishNet;
using FishNet.Transporting;
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
            InstanceFinder.ClientManager.StartConnection();

            yield return base.OnStateEnter();
        }

        public override IEnumerator OnStateExit()
        {
            if (InstanceFinder.NetworkManager.IsClient)
                PlayerManager.Instance.IsSpawnEnabled = false;

            yield return base.OnStateExit();
        }
    }
}