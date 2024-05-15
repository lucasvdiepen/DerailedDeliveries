using UnityEngine;

namespace DerailedDeliveries.Framework.TriggerArea
{
    /// <summary>
    /// A <see cref="TriggerAreaBase{T}"/> class that uses a <see cref="SphereCollider"/>.
    /// </summary>
    /// <typeparam name="T">The class to check for on collision.</typeparam>
    [RequireComponent(typeof(SphereCollider))]
    public class TriggerSphereArea<T> : TriggerAreaBase<T> where T : Component
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