using System;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.TriggerArea
{
    /// <summary>
    /// A class responsible for handling colliders entering and exiting a trigger area.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TriggerArea<T> : MonoBehaviour where T : Component
    {
        private readonly List<T> _componentsInCollider = new();

        /// <summary>
        /// Invoked when a collider enters or exits the trigger area.
        /// </summary>
        public Action<T[]> OnComponentChange;

        /// <summary>
        /// Invoked when a collider enters the trigger area.
        /// </summary>
        public Action<T> OnComponentEnter;

        /// <summary>
        /// Invoked when a collider exits the trigger area.
        /// </summary>
        public Action<T> OnComponentExit;

        /// <summary>
        /// Gets the colliders currently inside the trigger area.
        /// </summary>
        public T[] ComponentsInCollider => _componentsInCollider.ToArray();

        private void OnTriggerEnter(Collider other)
        {
            if(!other.TryGetComponent(out T component))
                return;

            if(_componentsInCollider.Contains(component))
                return;

            _componentsInCollider.Add(component);
            OnComponentEnter?.Invoke(component);
            OnComponentChange?.Invoke(_componentsInCollider.ToArray());
        }

        private void OnTriggerExit(Collider other)
        {
            if(!other.TryGetComponent(out T component))
                return;

            if(!_componentsInCollider.Contains(component))
                return;

            _componentsInCollider.Remove(component);
            OnComponentExit?.Invoke(component);
            OnComponentChange?.Invoke(_componentsInCollider.ToArray());
        }
    }

}