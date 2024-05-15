using UnityEngine;

using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.PopupManagement
{
    /// <summary>
    /// A class that is responsible for showing and hiding a popup when colliding with a player.
    /// </summary>
    public class PlayerPopupTriggerActivator : PopupTriggerActivator<PlayerId>
    {
        [Header("TriggerArea settings")]
        [SerializeField]
        private BoxCollider _collider;

        private protected virtual void Awake()
        {
            if (_collider == null)
                _collider = GetComponent<BoxCollider>();
        }

        private protected override Collider[] GetCollidingColliders()
            => Physics.OverlapBox(_collider.center + transform.position, GetColliderSize(_collider.size), transform.rotation);
    }
}