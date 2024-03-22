using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Player;
using DerailedDeliveries.Framework.Utils;
using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DerailedDeliveries.Framework.InputParser
{
    public class PlayerManager : NetworkAbstractSingleton<PlayerManager>
    {
        public Action<PlayerId> OnPlayerJoined;

        public Action<PlayerId> OnPlayerLeft;

        [SerializeField]
        private PlayerInputManager _playerInputManager;

        [SerializeField]
        private GameObject _playerPrefab;

        [SerializeField]
        private int _maxPlayers = 6;

        public int PlayerCount
        {
            get => _players.Count;
        }

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

        public override void OnStartClient()
        {
            base.OnStartClient();

            _playerInputManager.EnableJoining();
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            _playerInputManager.DisableJoining();
        }

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

        public void PlayerLeft(PlayerId playerId)
        {
            _players.Remove(playerId);
            OnPlayerLeft?.Invoke(playerId);
        }

        public PlayerId GetPlayer(int id)
        {
            PlayerId playerId = _players.FirstOrDefault(player => player.Id == id);
            return playerId;
        }

        public void SpawnPlayer(NetworkConnection connection, PlayerSpawner playerSpawner)
        {
            if(_playerSpawners.Contains(playerSpawner))
                return;

            _playerSpawners.Add(playerSpawner);

            SpawnPlayerOnServer(connection);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerOnServer(NetworkConnection connection)
        {
            GameObject spawnedPlayer = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
            NetworkObject networkObject = spawnedPlayer.GetComponent<NetworkObject>();

            ServerManager.Spawn(spawnedPlayer, connection);
            SceneManager.AddOwnerToDefaultScene(networkObject);

            spawnedPlayer.GetComponent<PlayerId>().SetId(_playerIdCount);
            _playerIdCount++;
        }

        public void DespawnPlayer(NetworkObject networkObject) => DespawnPlayerOnServer(networkObject);

        [ServerRpc(RequireOwnership = false)]
        private void DespawnPlayerOnServer(NetworkObject networkObject)
        {
            ServerManager.Despawn(networkObject);
        }
    }
}