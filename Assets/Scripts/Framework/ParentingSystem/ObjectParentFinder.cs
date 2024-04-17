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
        }

        private void SetOrUnsetParent(ObjectParent objectParent, bool isEntering)
        {
            if (isEntering)
            {
                objectParent.SetParent(NetworkObject);
                _collidingParents.Add(objectParent);
                return;
            }

            _collidingParents.Remove(objectParent);

            // If the parent is unset, check if there are any other parents inside collidingParents.
            if (_collidingParents.Count > 0)
            {
                _collidingParents[^1].SetParent(NetworkObject);
                return;
            }

            NetworkObject.UnsetParent();
        }
    }
}