using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

using DerailedDeliveries.Framework.Utils;

namespace DerailedDeliveries.Framework.PlayerManagement
{
    /// <summary>
    /// A class that manages all the networked players in the game.
    /// </summary>
    public class PlayerManager : NetworkAbstractSingleton<PlayerManager>
    {
        [SerializeField]
        private PlayerInputManager _playerInputManager;

        [SerializeField]
        private GameObject _playerPrefab;

        [SerializeField]
        private int _maxPlayers = 6;

        /// <summary>
        /// Invoked when a player joins the game. The PlayerId script is passed as an argument.
        /// </summary>
        public Action<PlayerId> OnPlayerJoined;

        /// <summary>
        /// Invoked when a player leaves the game.
        /// </summary>
        public Action OnPlayerLeft;

        /// <summary>
        /// The amount of players currently in the game.
        /// </summary>
        public int PlayerCount => _players.Count;

        /// <summary>
        /// Whether spawning new players is enabled.
        /// </summary>
        public bool IsSpawnEnabled
        {
            get
            {
                return _isSpawnEnabled;
            }
            set
            {
                _isSpawnEnabled = value;

                if(value)
                    _playerInputManager.EnableJoining();
                else
                    _playerInputManager.DisableJoining();
            }
        }

        private readonly List<PlayerId> _players = new();
        private readonly List<PlayerSpawner> _playerSpawners = new();
        private bool _isSpawnEnabled;
        private int _playerIdCount;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            _playerInputManager.EnableJoining();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStopClient()
        {
            base.OnStopClient();

            _playerInputManager.DisableJoining();
        }

        /// <summary>
        /// Called when a player joined the game.
        /// </summary>
        /// <param name="playerId">The PlayerId that joined the game.</param>
        public void PlayerJoined(PlayerId playerId)
        {
            _players.Add(playerId);

            if(playerId.Owner.IsLocalClient && _playerSpawners.Count > 0)
            {
                PlayerSpawner playerSpawner = _playerSpawners[0];

                PlayerInput playerSpawnerInput = playerSpawner.GetComponent<PlayerInput>();
                PlayerInput playerInput = playerId.GetComponent<PlayerInput>();

                playerInput.SwitchCurrentControlScheme(playerSpawnerInput.currentControlScheme, playerSpawnerInput.devices.ToArray());

                Destroy(playerSpawner.gameObject);
                _playerSpawners.Remove(playerSpawner);
            }

            OnPlayerJoined?.Invoke(playerId);
        }

        /// <summary>
        /// Called when a player left the game.
        /// </summary>
        /// <param name="playerId">The PlayerId that left the game.</param>
        public void PlayerLeft(PlayerId playerId)
        {
            _players.Remove(playerId);
            OnPlayerLeft?.Invoke();
        }

        /// <summary>
        /// Gets a player by their id.
        /// </summary>
        /// <param name="id">The id of the player to get.</param>
        /// <returns>The PlayerId script of the player with the given id.</returns>
        public PlayerId GetPlayer(int id)
        {
            PlayerId playerId = _players.FirstOrDefault(player => player.Id == id);
            return playerId;
        }

        /// <summary>
        /// Tries to spawn a player on the server.
        /// </summary>
        /// <param name="clientConnection">The client connection that wants to spawn a player.</param>
        /// <param name="playerSpawner">The player spawner which is trying to spawn a player.</param>
        public void SpawnPlayer(NetworkConnection clientConnection, PlayerSpawner playerSpawner)
        {
            if(_players.Count >= _maxPlayers)
            {
                Destroy(playerSpawner.gameObject);
                return;
            }

            if(_playerSpawners.Contains(playerSpawner))
                return;

            _playerSpawners.Add(playerSpawner);

            SpawnPlayerOnServer(clientConnection);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerOnServer(NetworkConnection clientConnection)
        {
            if(_players.Count >= _maxPlayers)
                return;

            GameObject spawnedPlayer = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
            NetworkObject networkObject = spawnedPlayer.GetComponent<NetworkObject>();

            ServerManager.Spawn(spawnedPlayer, clientConnection);
            SceneManager.AddOwnerToDefaultScene(networkObject);

            spawnedPlayer.GetComponent<PlayerId>().SetId(_playerIdCount);
            _playerIdCount++;
        }

        /// <summary>
        /// Despawns a player on the server.
        /// </summary>
        /// <param name="networkObject">The network object of the player to despawn.</param>
        public void DespawnPlayer(NetworkObject networkObject) => DespawnPlayerOnServer(networkObject);

        [ServerRpc(RequireOwnership = false)]
        private void DespawnPlayerOnServer(NetworkObject networkObject)
        {
            ServerManager.Despawn(networkObject);
        }
    }
}