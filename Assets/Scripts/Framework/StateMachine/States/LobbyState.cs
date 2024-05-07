using System.Collections;
using FishNet;
using FishNet.Transporting;
using UnityEngine.SceneManagement;

using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.StateMachine.States
{
    /// <summary>
    /// The state that represents the lobby menu.
    /// </summary>
    public class LobbyState : MenuState
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
            PlayerManager.Instance.IsSpawningEnabled = false;
            
            InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnnectionStateChanged;

            yield return base.OnStateExit();
        }

        private void OnClientConnnectionStateChanged(ClientConnectionStateArgs args)
        {
            if(args.ConnectionState != LocalConnectionState.Stopped)
                return;

            //StateMachine.Instance.GoToState<MainMenuState>();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}