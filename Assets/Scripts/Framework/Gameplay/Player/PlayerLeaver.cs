using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.InputParser;
using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    /// <summary>
    /// A class responsible for despawning the player when the leave button is pressed.
    /// </summary>
    [RequireComponent(typeof(PlayerLobbyInputParser))]
    public class PlayerLeaver : NetworkBehaviour
    {
        private PlayerLobbyInputParser _playerLobbyInputParser;

        private void Awake() => _playerLobbyInputParser = GetComponent<PlayerLobbyInputParser>();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            if(!IsOwner)
                return;

            _playerLobbyInputParser.OnLeave += DespawnPlayer;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStopClient()
        {
            base.OnStopClient();

            if(!IsOwner)
                return;

            _playerLobbyInputParser.OnLeave -= DespawnPlayer;
        }

        private void DespawnPlayer() => PlayerManager.Instance.DespawnPlayer(NetworkObject);
    }
}