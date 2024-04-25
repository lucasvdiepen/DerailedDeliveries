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

        private ObjectParent _currentParent;

        private void FixedUpdate()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position + _sphereCollider.center, _sphereCollider.radius, _collisionLayer);

            List<ObjectParent> collidingParents = GetAllObjectParents(colliders);

            SetOrUnsetParent(collidingParents);
        }

        private List<ObjectParent> GetAllObjectParents(Collider[] colliders)
        {
            List<ObjectParent> collidingParents = new();
            foreach(Collider collider in colliders)
            {
                if(!ObjectParentUtils.TryGetObjectParent(collider.gameObject, out ObjectParent objectParent))
                    continue;

                if(collidingParents.Contains(objectParent))
                    continue;

                collidingParents.Add(objectParent);
            }

            return collidingParents;
        }

        private void SetOrUnsetParent(List<ObjectParent> collidingParents)
        {
            if(collidingParents.Count > 0)
            {
                if(collidingParents.Contains(_currentParent))
                    return;

                ObjectParent objectParent = collidingParents[0];

                _currentParent = objectParent;
                objectParent.SetParent(NetworkObject);

                return;
            }

            _currentParent = null;
            NetworkObject.UnsetParent();
        }
    }
}