using FishNet;
using FishNet.Transporting;
using System.Collections;
using UnityEngine.SceneManagement;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the game.
    /// </summary>
    public class GameState : MenuState
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateEnter()
        {
            yield return base.OnStateEnter();

            InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnnectionStateChanged;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override IEnumerator OnStateExit()
        {
            InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnnectionStateChanged;

            yield return base.OnStateExit();
        }

        private void OnClientConnnectionStateChanged(ClientConnectionStateArgs args)
        {
            if(args.ConnectionState != LocalConnectionState.Stopped)
                return;

            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}