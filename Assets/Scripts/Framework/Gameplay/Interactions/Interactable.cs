using System.Collections;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.Gameplay.Player;
using FishNet.Object;
using FishNet.Observing;

namespace DerailedDeliveries.Framework.Gameplay.Interactions
{
    /// <summary>
    /// A class that is responsible for holding values for an interactable object.
    /// </summary>
    [RequireComponent(typeof(NetworkObject), typeof(NetworkObserver))]
    public class Interactable : MonoBehaviour
    {
        /// <summary>
        /// Gets called when a player interacts with this interactable.
        /// </summary>
        public Action OnInteract;

        [SerializeField]
        private float _cooldown = .5f;

        private bool _isOnCooldown;

        private bool _interactable;

        private protected bool IsInteractable
        {
            get => _interactable;
            set => _interactable = value;
        }

        /// <summary>
        /// The base function for any Interactable that is used to call the functionality of the Interactable.
        /// Can be overwritten in classes that derive from the base class.
        /// </summary>
        [ServerRpc]
        public virtual void Interact(Interactor interactor)
        {
            if (!_interactable || _isOnCooldown)
                return;

            StartCoroutine(ActivateCooldown());

            OnInteract?.Invoke();
        }

        private protected virtual IEnumerator ActivateCooldown()
        {
            _isOnCooldown = false;
            yield return new WaitForSeconds(_cooldown);
            _isOnCooldown = true;
        }
    }
}