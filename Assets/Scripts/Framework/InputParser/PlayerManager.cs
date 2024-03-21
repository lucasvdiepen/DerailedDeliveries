using DerailedDeliveries.Framework.Utils;
using FishNet.Connection;
using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DerailedDeliveries.Framework.InputParser
{
    public class PlayerManager : NetworkAbstractSingleton<PlayerManager>
    {
        [SerializeField]
        private PlayerInputManager _playerInputManager;

        [SerializeField]
        private GameObject _playerPrefab;

        [SerializeField]
        private int _maxPlayers = 6;

        public int PlayerCount { get; private set; }

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

        private readonly List<NetworkObject> _players = new();
        private bool _isSpawnEnabled;

        public void SpawnPlayer(NetworkConnection connection) => SpawnPlayerOnServer(connection);

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerOnServer(NetworkConnection connection)
        {
            GameObject spawnedPlayer = Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity);
            ServerManager.Spawn(spawnedPlayer, connection);

            PlayerCount++;
        }

        [ServerRpc(RequireOwnership = false)]
        private void DespawnPlayer(NetworkObject networkObject)
        {
            _players.Remove(networkObject);
            ServerManager.Despawn(networkObject);

            PlayerCount--;
        }

        [ObserversRpc]
        private void OnPlayerJoined()
        {

        }

        [ObserversRpc]
        private void OnPlayerLeft()
        {

        }
    }
}