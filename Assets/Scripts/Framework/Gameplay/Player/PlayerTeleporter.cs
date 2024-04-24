using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    /// <summary>
    /// A class that teleports the player back to the spawn point if they are too far away from the spawn point.
    /// </summary>
    public class PlayerTeleporter : NetworkBehaviour
    {
        [SerializeField]
        private float _maxDistance = 50;

        private Transform _teleportTarget;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            _teleportTarget = PlayerManager.Instance.SpawnPoint;
        }

        private void LateUpdate()
        {
            if (Vector3.Distance(transform.position, _teleportTarget.position) <= _maxDistance)
                return;

            transform.position = _teleportTarget.position;
        }
    }
}