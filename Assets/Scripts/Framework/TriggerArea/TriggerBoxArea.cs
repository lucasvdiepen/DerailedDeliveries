using UnityEngine;

namespace DerailedDeliveries.Framework.TriggerArea
{
    /// <summary>
    /// A <see cref="TriggerAreaBase{T}"/> class that uses a <see cref="BoxCollider"/>.
    /// </summary>
    /// <typeparam name="T">The class to check for on collision.</typeparam>
    public class TriggerBoxArea<T> : TriggerAreaBase<T> where T : Component
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
            => Physics.OverlapBox(transform.position + _collider.center, _collider.size);
    }
}