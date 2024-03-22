using FishNet;
using UnityEngine;

using DerailedDeliveries.Framework.InputParser;

namespace DerailedDeliveries.Framework.PlayerManagement
{
    /// <summary>
    /// A class responsible for spawning the player.
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        private void Start() => PlayerManager.Instance.SpawnPlayer(InstanceFinder.ClientManager.Connection, this);
    }
}