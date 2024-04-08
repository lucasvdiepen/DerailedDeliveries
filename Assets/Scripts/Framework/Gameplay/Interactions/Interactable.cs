using FishNet.Object.Synchronizing;
using System.Collections;
using FishNet.Observing;
using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;

namespace DerailedDeliveries.Framework.Gameplay.Interactions
{
    /// <summary>
    /// A class that is responsible for holding values for an interactable object.
    /// </summary>
    [RequireComponent(typeof(NetworkObject), typeof(NetworkObserver), typeof(BoxCollider))]
    public abstract class Interactable : NetworkBehaviour
    {
        [SerializeField]
        private float _cooldown = .5f;

        /// <summary>
        /// A getter that returns this interactable's <see cref="UnityEngine.BoxCollider"/>.
        /// </summary>
        public BoxCollider BoxCollider { get; private set; }

        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private protected bool IsOnCooldown { get; set; }

        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private protected bool IsInteractable { get; set; } = true;

        private protected virtual void Awake() => BoxCollider = GetComponent<BoxCollider>();

        /// <summary>
        /// Returns a boolean that reflects if this Interactable is available for interaction.
        /// </summary>
        /// <param name="interactor">The interactor that is checking if this is interactable.</param>
        /// <returns>The status that reflects if this is interactable.</returns>
        public virtual bool CheckIfInteractable(Interactor interactor) => IsInteractable && !IsOnCooldown;

        /// <summary>
        /// A function that calls a RPC to the server on this Interactable.
        /// </summary>
        /// <param name="interactor">The interactor that this request originates from.</param>
        [ServerRpc(RequireOwnership = false)]
        public void InteractOnServer(Interactor interactor) => Interact(interactor);

        /// <summary>
        /// A function that calls the Interact function when already on the server.
        /// </summary>
        /// <param name="interactor">The origin interactor this request originates from.</param>
        /// <returns>The result of if the interaction was succesfull.</returns>
        [Server]
        public bool InteractAsServer(Interactor interactor) => Interact(interactor);

        [Server]
        private protected virtual bool Interact(Interactor interactor)
        {
            if(!IsInteractable || IsOnCooldown)
                return false;

            StartCoroutine(ActivateCooldown());

            return true;
        }

        /// <summary>
        /// An interact function that is called from interactable to interactable.
        /// </summary>
        /// <param name="interactor">The origin Interactor.</param>
        /// <returns>The status of if the Interaction was succesfull.</returns>
        [Server]
        public virtual bool Interact(UseableGrabbable useableGrabbable) => false;

        private protected virtual IEnumerator ActivateCooldown()
        {
            IsOnCooldown = true;
            yield return new WaitForSeconds(_cooldown);
            IsOnCooldown = false;
        }
    }
}