using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.ParentingSystem
{
    public class ObjectParentFinder : NetworkBehaviour
    {
        private List<ObjectParent> _collidingParents = new();

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.transform.TryGetComponent(out ObjectParent objectParent))
            {
                objectParent.SetParent(NetworkObject);
                _collidingParents.Add(objectParent);
                return;
            }

            ObjectParent parentObjectParent = collision.transform.GetComponentInParent<ObjectParent>();
            if(parentObjectParent == null)
                return;

            parentObjectParent.SetParent(NetworkObject);
            _collidingParents.Add(parentObjectParent);
        }

        private void OnCollisionExit(Collision collision)
        {
            if(collision.transform.TryGetComponent(out ObjectParent objectParent))
            {
                NetworkObject.UnsetParent();
                _collidingParents.Remove(objectParent);
                return;
            }

            ObjectParent parentObjectParent = collision.transform.GetComponentInParent<ObjectParent>();
            if(parentObjectParent == null)
                return;

            NetworkObject.UnsetParent();
            _collidingParents.Remove(parentObjectParent);

            foreach(ObjectParent parent in _collidingParents)
                parent.SetParent(NetworkObject);
        }
    }
}