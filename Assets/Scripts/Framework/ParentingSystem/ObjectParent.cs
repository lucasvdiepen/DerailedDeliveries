using FishNet.Object;
using FishNet.Observing;
using UnityEngine;

namespace DerailedDeliveries.Framework.ParentingSystem
{
    /// <summary>
    /// A class responsible for setting the parent of an object.
    /// </summary>
    [RequireComponent(typeof(NetworkObject), typeof(NetworkObserver))]
    public class ObjectParent : NetworkBehaviour
    {
        /// <summary>
        /// Sets the parent of the given object to this object.
        /// </summary>
        /// <param name="networkObject">The object to set the parent of.</param>
        public void SetParent(NetworkObject networkObject) => networkObject.SetParent(this);
    }
}