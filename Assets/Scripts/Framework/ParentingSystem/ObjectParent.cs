using FishNet.Object;
using UnityEngine;

namespace DerailedDeliveries.Framework.ParentingSystem
{
    [RequireComponent(typeof(EmptyNetworkBehaviour))]
    public class ObjectParent : MonoBehaviour
    {
        private NetworkBehaviour _networkBehaviour;

        private void Awake() => _networkBehaviour = GetComponent<NetworkBehaviour>();

        public void SetParent(NetworkObject networkObject) => networkObject.SetParent(_networkBehaviour);
    }
}