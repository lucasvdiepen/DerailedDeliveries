using DerailedDeliveries.Framework.InputParser;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    public class PlayerId : NetworkBehaviour
    {
        public int Id { get; private set; }

        public override void OnStartClient()
        {
            base.OnStartClient();

            PlayerManager.Instance.PlayerJoined(this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            PlayerManager.Instance.PlayerLeft(this);
        }

        [ObserversRpc(BufferLast = true)]
        public void SetId(int id) => Id = id;
    }
}