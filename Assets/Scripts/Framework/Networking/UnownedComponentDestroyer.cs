using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace DerailedDeliveries.Framework.Networking
{
    /// <summary>
    /// A class responsible for destroying components when the client does not own the object.
    /// </summary>
    public class UnownedComponentDestroyer : NetworkBehaviour
    {
        [SerializeField]
        private MonoBehaviour[] _components;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);

            DestroyAll();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            DestroyAll();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);

            DestroyAll();
        }

        private void DestroyAll()
        {
            if (Owner.IsLocalClient)
                return;

            foreach (MonoBehaviour component in _components)
                Destroy(component);

            Destroy(this);
        }
    }
}