using DerailedDeliveries.Framework.Utils;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.InputParser
{
    public class PlayerManager : NetworkAbstractSingleton<PlayerManager>
    {
        public int PlayerCount { get; private set; }

        [ServerRpc]
        public void SpawnPlayer()
        {

        }
    }
}