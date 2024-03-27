using FishNet.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.TriggerArea
{
    /// <summary>
    /// A class responsible for handling colliders entering and exiting a trigger area.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class NetworkTriggerArea<T> : NetworkBehaviour where T : Component
    {
        private readonly List<T> _colliders = new();

        /// <summary>
        /// Invoked when a collider enters or exits the trigger area.
        /// </summary>
        public Action<T[]> OnColliderChange;

        /// <summary>
        /// Invoked when a collider enters the trigger area.
        /// </summary>
        public Action<T> OnColliderEnter;

        /// <summary>
        /// Invoked when a collider exits the trigger area.
        /// </summary>
        public Action<T> OnColliderExit;

        /// <summary>
        /// Gets the colliders currently inside the trigger area.
        /// </summary>
        public T[] ComponentsInCollider => _colliders.ToArray();

        private void OnTriggerEnter(Collider other)
        {
            if(!other.TryGetComponent(out T component))
                return;

            if(_colliders.Contains(component))
                return;

            _colliders.Add(component);
            OnColliderEnter?.Invoke(component);
            OnColliderChange?.Invoke(_colliders.ToArray());
        }

        private void OnTriggerExit(Collider other)
        {
            if(!other.TryGetComponent(out T component))
                return;

            if(!_colliders.Contains(component))
                return;

            _colliders.Remove(component);
            OnColliderExit?.Invoke(component);
            OnColliderChange?.Invoke(_colliders.ToArray());
        }
    }

}