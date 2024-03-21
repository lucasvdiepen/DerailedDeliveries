using DerailedDeliveries.Framework.InputParser;
using FishNet;
using UnityEngine;

namespace DerailedDeliveries.Framework.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        private void Start()
        {
            PlayerManager.Instance.SpawnPlayer(InstanceFinder.ClientManager.Connection);
        }
    }
}