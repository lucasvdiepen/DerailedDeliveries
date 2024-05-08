using DerailedDeliveries.Framework.Gameplay.Interactions;
using UnityEngine;

namespace DerailedDeliveries.Framework.PopupManagement
{
    /// <summary>
    /// A class that is responsible for showing and hiding a popup when colliding with an interactable.
    /// </summary>
    public class InteractablePopupTriggerActivator : PopupTriggerActivator<Interactable>
    {
        [Header("TriggerArea settings")]
        [SerializeField]
        private SphereCollider _collider;

        private protected virtual void Awake()
        {
            if (_collider == null)
                _collider = GetComponent<SphereCollider>();
        }

        private protected override Collider[] GetCollidingColliders()
            => Physics.OverlapSphere((transform.rotation * _collider.center) + transform.position, _collider.radius);
    }
}