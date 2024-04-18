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
        [SerializeField]
        private SphereCollider _sphereCollider;

        [SerializeField]
        private LayerMask _collisionLayer;

        private readonly List<ObjectParent> _collidingParents = new();
        private ObjectParent _currentParent;

        private void FixedUpdate()
        {
            List<ObjectParent> currentCollidingParents = new();
            Collider[] colliders = Physics.OverlapSphere(transform.position + _sphereCollider.center, _sphereCollider.radius, _collisionLayer);
            foreach(Collider collider in colliders)
            {
                if (!ObjectParentUtils.TryGetObjectParent(collider.gameObject, out ObjectParent objectParent))
                    continue;

                Debug.Log("Collider with object parent: " + collider.gameObject.name);

                if(currentCollidingParents.Contains(objectParent))
                    continue;

                currentCollidingParents.Add(objectParent);
            }

            Debug.Log("collider count: " + colliders.Length);
            Debug.Log("object parent count: " + currentCollidingParents.Count);

            if(currentCollidingParents.Count > 0)
            {
                if(currentCollidingParents.Contains(_currentParent))
                    return;

                NetworkObject.UnsetParent();
                currentCollidingParents[0].SetParent(NetworkObject);
                _currentParent = currentCollidingParents[0];
                return;
            }

            _currentParent = null;
            NetworkObject.UnsetParent();
        }

        private void SetOrUnsetParent(ObjectParent objectParent, bool isEntering)
        {
            if (isEntering)
            {
                objectParent.SetParent(NetworkObject);
                //_collidingParents.Add(objectParent);
                return;
            }

            //_collidingParents.Remove(objectParent);

            // If the parent is unset, check if there are any other parents inside collidingParents.
            /*if (_collidingParents.Count > 0)
            {
                _collidingParents[^1].SetParent(NetworkObject);
                return;
            }*/

            NetworkObject.UnsetParent();
        }
    }
}