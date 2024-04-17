using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.ParentingSystem
{
    /// <summary>
    /// A class responsible for finding parents for this object.
    /// </summary>
    public class ObjectParentFinder : NetworkBehaviour
    {
        private List<ObjectParent> _collidingParents = new();

        private void OnCollisionEnter(Collision collision)
        {
            if (SetOrUnsetParent(collision, true))
                return;

            SetOrUnsetInParents(collision, true);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (SetOrUnsetParent(collision, false))
                return;

            if(!SetOrUnsetInParents(collision, false))
                return;

            // If the parent is unset, check if there are any other parents inside collidingParents.
            foreach(ObjectParent parent in _collidingParents)
                parent.SetParent(NetworkObject);
        }

        private bool SetOrUnsetInParents(Collision collision, bool isEntering)
        {
            ObjectParent parentObjectParent = collision.transform.GetComponentInParent<ObjectParent>();
            if(parentObjectParent == null)
                return false;

            SetOrUnsetParent(parentObjectParent, isEntering);

            return true;
        }

        private bool SetOrUnsetParent(Collision collision, bool isEntering)
        {
            bool state = collision.transform.TryGetComponent(out ObjectParent objectParent);

            if (!state)
                return false;

            SetOrUnsetParent(objectParent, isEntering);

            return true;
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