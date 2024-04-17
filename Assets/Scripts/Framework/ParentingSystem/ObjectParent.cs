using FishNet.Object;
using UnityEngine;

namespace DerailedDeliveries.Framework.ParentingSystem
{
    /// <summary>
    /// A class responsible for setting the parent of an object.
    /// </summary>
    [RequireComponent(typeof(EmptyNetworkBehaviour))]
    public class ObjectParent : MonoBehaviour
    {
        private NetworkBehaviour _networkBehaviour;

        private void Awake() => _networkBehaviour = GetComponent<NetworkBehaviour>();

        /// <summary>
        /// Sets the parent of the given object to this object.
        /// </summary>
        /// <param name="networkObject">The object to set the parent of.</param>
        public void SetParent(NetworkObject networkObject) => networkObject.SetParent(_networkBehaviour);
    }
}