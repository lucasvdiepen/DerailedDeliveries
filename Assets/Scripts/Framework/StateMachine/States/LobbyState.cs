using System.Collections;

using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the lobby menu.
    /// </summary>
    public class LobbyState : MenuState
    {
        public override IEnumerator OnStateExit()
        {
            yield return base.OnStateExit();

            PlayerManager.Instance.IsSpawnEnabled = false;
        }
    }
}