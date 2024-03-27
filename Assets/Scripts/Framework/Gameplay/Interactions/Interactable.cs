using System.Collections;
using FishNet.Observing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions
{
    /// <summary>
    /// A class that is responsible for holding values for an interactable object.
    /// </summary>
    [RequireComponent(typeof(NetworkObject), typeof(NetworkObserver), typeof(BoxCollider))]
    public class Interactable : NetworkBehaviour
    {
        /// <summary>
        /// Gets called when a player interacts with this interactable.
        /// </summary>
        public Action OnInteract;

        [SerializeField]
        private float _cooldown = .5f;

        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private protected bool IsOnCooldown { get; set; }

        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private protected bool IsInteractable { get; set; } = true;

        /// <summary>
        /// Returns a boolean that reflects if this Interactable is available for interaction.
        /// </summary>
        /// <returns>The status that reflects if this is interactable.</returns>
        public virtual bool CheckIfInteractable() => IsInteractable && !IsOnCooldown;

        /// <summary>
        /// A function that calls a RPC to the server on this Interactable.
        /// </summary>
        /// <param name="interactor">The interactor that this request originates from.</param>
        [ServerRpc(RequireOwnership = false), Server]
        public void InteractOnServer(Interactor interactor) => Interact(interactor);

        [Server]
        private protected virtual bool Interact(Interactor interactor)
        {
            if(!IsInteractable || IsOnCooldown)
                return false;

            StartCoroutine(ActivateCooldown());

            OnInteract?.Invoke();

            return true;
        }

        private protected virtual IEnumerator ActivateCooldown()
        {
            IsOnCooldown = true;
            yield return new WaitForSeconds(_cooldown);
            IsOnCooldown = false;
        }
    }
}