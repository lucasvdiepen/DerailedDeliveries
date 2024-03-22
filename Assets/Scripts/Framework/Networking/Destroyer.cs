using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace DerailedDeliveries.Framework.Networking
{
    public class Destroyer : NetworkBehaviour
    {
        [SerializeField]
        private MonoBehaviour[] _components;

        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);

            DestroyAll();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            DestroyAll();
        }

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