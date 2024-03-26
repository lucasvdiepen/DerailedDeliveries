using FishNet;
using UnityEngine;

namespace DerailedDeliveries.Framework.PlayerManagement
{
    /// <summary>
    /// A class responsible for requesting the spawning of the player to the server.
    /// </summary>
    public class PlayerSpawnRequester : MonoBehaviour
    {
        private void Start() => PlayerManager.Instance.SpawnPlayer(InstanceFinder.ClientManager.Connection, this);
    }
}