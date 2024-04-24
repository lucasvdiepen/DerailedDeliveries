using System.Collections;
using FishNet;

using DerailedDeliveries.Framework.Gameplay.Timer;
using FishNet.Transporting;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the game.
    /// </summary>
    public class GameState : MenuState
    {
        public override IEnumerator OnStateEnter()
        {
            if (TimerUpdater.Instance.IsServer)
                TimerUpdater.Instance.OnTimerCompleted += StopConnection;
            else
                InstanceFinder.ClientManager.OnClientConnectionState += ReturnToMenuState;

            return base.OnStateEnter();
        }

        public override IEnumerator OnStateExit()
        {
            if (TimerUpdater.Instance.IsServer)
                TimerUpdater.Instance.OnTimerCompleted -= StopConnection;
            else
                InstanceFinder.ClientManager.OnClientConnectionState -= ReturnToMenuState;

            return base.OnStateExit();
        }

        private void StopConnection()
        {
            InstanceFinder.ServerManager.StopConnection(true);

            StateMachine.Instance.GoToState<MainMenuState>();
        }

        private void ReturnToMenuState(ClientConnectionStateArgs conn)
        {
            if(conn.ConnectionState == LocalConnectionState.Stopped 
               || conn.ConnectionState == LocalConnectionState.Stopping)
                StateMachine.Instance.GoToState<MainMenuState>();
        }
    }
}