using Component = UnityEngine.Component;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using System;

namespace DerailedDeliveries.Framework.TriggerArea
{
    /// <summary>
    /// The base class for all TriggerArea classes.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class TriggerAreaBase<T> : MonoBehaviour where T : Component
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

        [SerializeField]
        private int _framesUntillUpdate = 25;

        private int _framesPassedSinceUpdate = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out T component))
                return;

            AddNewComponent(component);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out T component))
                return;

            RemoveOldComponent(component);
        }

        private void AddNewComponent(T component)
        {
            if (_colliders.Contains(component))
                return;

            _colliders.Add(component);
            OnColliderEnter?.Invoke(component);
            OnColliderChange?.Invoke(_colliders.ToArray());
        }

        private void RemoveOldComponent(T component)
        {
            if (!_colliders.Contains(component))
                return;

            _colliders.Remove(component);
            OnColliderExit?.Invoke(component);
            OnColliderChange?.Invoke(_colliders.ToArray());
        }

        private protected virtual void FixedUpdate()
        {
            if (_framesPassedSinceUpdate % _framesUntillUpdate != 0)
            {
                _framesPassedSinceUpdate++;
                return;
            }

            _framesPassedSinceUpdate = 0;

            List<T> newColliding = GetCollidingColliders().Select(collider => collider.GetComponent<T>()).ToList();
            int newCollidingCount = newColliding.Count;

            for (int i = 0; i < newCollidingCount; i++)
                if (newColliding[i] != null)
                    AddNewComponent(newColliding[i]);

            for (int i = 0; i < _colliders.Count; i++)
                if (!newColliding.Contains(_colliders[i]))
                    RemoveOldComponent(_colliders[i]);
        }

        private protected abstract Collider[] GetCollidingColliders();
    }
}