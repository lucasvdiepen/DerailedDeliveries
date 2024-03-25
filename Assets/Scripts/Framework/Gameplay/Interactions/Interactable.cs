using System.Collections;
using FishNet.Observing;
using FishNet.Object;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions
{
    /// <summary>
    /// A class that is responsible for holding values for an interactable object.
    /// </summary>
    [RequireComponent(typeof(NetworkObject), typeof(NetworkObserver))]
    public class Interactable : NetworkBehaviour
    {
        /// <summary>
        /// Gets called when a player interacts with this interactable.
        /// </summary>
        public Action OnInteract;

        [SerializeField]
        private float _cooldown = .5f;

        private protected bool IsOnCooldown { get; set; }

        private protected bool IsInteractable { get; set; } = true;

        private protected bool IsBeingInteracted { get; set; }

        /// <summary>
        /// A function that calls a RPC to the server on this Interactable.
        /// </summary>
        /// <param name="interactor">The interactor that this request originates from.</param>
        [ServerRpc(RequireOwnership = false)]
        public void InteractOnServer(Interactor interactor)
        {
            if (!IsInteractable || IsOnCooldown)
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
            IsOnCooldown = true;
            yield return new WaitForSeconds(_cooldown);
            IsOnCooldown = false;
        }
    }
}