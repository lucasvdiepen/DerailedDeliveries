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

        private bool _isBeingInteracted;

        private protected bool IsInteractable
        {
            get => _interactable;
            set => _interactable = value;
        }
        private protected bool IsBeingInteracted
        {
            get => _isBeingInteracted;
            set => _isBeingInteracted = value;
        }

        /// <summary>
        /// A function that calls a RPC to the server on this Interactable.
        /// </summary>
        /// <param name="interactor">The interactor that his request originates from.</param>
        [ServerRpc]
        public void InteractOnServer(Interactor interactor)
        {
            if (!_interactable || _isOnCooldown || _isBeingInteracted)
                return;

            Interact(interactor);
        }

        private protected virtual void Interact(Interactor interactor)
        {
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