using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

using DerailedDeliveries.Framework.Utils.ObjectParenting;

namespace DerailedDeliveries.Framework.ParentingSystem
{
    /// <summary>
    /// A class responsible for finding parents for this object.
    /// </summary>
    public class ObjectParentFinder : NetworkBehaviour
    {
        private readonly List<ObjectParent> _collidingParents = new();

        private void OnCollisionEnter(Collision collision)
        {
            if(!ObjectParentUtils.TryGetObjectParent(collision.gameObject, out ObjectParent objectParent))
                return;

            SetOrUnsetParent(objectParent, true);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!ObjectParentUtils.TryGetObjectParent(collision.gameObject, out ObjectParent objectParent))
                return;

            SetOrUnsetParent(objectParent, false);

            // If the parent is unset, check if there are any other parents inside collidingParents.
            foreach(ObjectParent parent in _collidingParents)
                parent.SetParent(NetworkObject);
        }

        private void SetOrUnsetParent(ObjectParent objectParent, bool isEntering)
        {
            if (isEntering)
            {
                objectParent.SetParent(NetworkObject);
                _collidingParents.Add(objectParent);
                return;
            }

            NetworkObject.UnsetParent();
            _collidingParents.Remove(objectParent);
        }
    }
}