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
        /// A function that returns all colliding colliders.
        /// </summary>
        /// <returns>An array of colliding <see cref="Collider"/>'s.</returns>
        [Server]
        public Collider[] GetCollidingColliders() =>
            Physics.OverlapBox(BoxCollider.center + transform.position, BoxCollider.size);

        /// <summary>
        /// Returns a boolean that reflects if this <see cref="Interactable"/> is available for interaction.
        /// </summary>
        /// <param name="interactor">The <see cref="Interactor"/> that is checking if this is interactable.</param>
        /// <returns>The status that reflects if this is interactable.</returns>
        public virtual bool CheckIfInteractable(Interactor interactor) => IsInteractable && !IsOnCooldown;

        /// <summary>
        /// A function that calls a RPC to the server on this <see cref="Interactable"/>.
        /// </summary>
        /// <param name="interactor">The <see cref="Interactor"/> that this request originates from.</param>
        [ServerRpc(RequireOwnership = false)]
        public void InteractOnServer(Interactor interactor) => Interact(interactor);

        /// <summary>
        /// A function that calls the Interact function when already on the server.
        /// </summary>
        /// <param name="interactor">The origin <see cref="Interactor"/> this request originates from.</param>
        /// <returns>The result of if the interaction was succesfull.</returns>
        [Server]
        public bool InteractAsServer(Interactor interactor) => Interact(interactor);

        /// <summary>
        /// A function that calls <see cref="Use(Interactor)"/> method on the server using a RPC.
        /// </summary>
        /// <param name="interactor">The <see cref="Interactor"/> that this request originates from.</param>
        [ServerRpc(RequireOwnership = false)]
        public void UseOnServer(Interactor interactor) => Use(interactor);

        /// <summary>
        /// A function that calls the <see cref="Use(Interactor)"/> function when already on the server.
        /// </summary>
        /// <param name="interactor">The <see cref="Interactor"/> that this request originates from.</param>
        /// <returns>The result of if the use interaction was succesfull.</returns>
        public bool UseAsServer(Interactor interactor) => Use(interactor);

        [Server]
        private protected virtual bool Interact(Interactor interactor)
        {
            if(!IsInteractable || IsOnCooldown)
                return false;

            StartCoroutine(ActivateCooldown());

            return true;
        }

        /// <summary>
        /// An interact function that is called from <see cref="Interactable"/> to <see cref="Interactable"/>.
        /// </summary>
        /// <param name="interactor">The origin <see cref="Interactable"/>.</param>
        /// <returns>The status of if the Interaction was succesfull.</returns>
        [Server]
        public virtual bool Interact(UseableGrabbable useableGrabbable) => false;

        [Server]
        private protected virtual bool Use(Interactor interactor)
        {
            if (!IsInteractable || IsOnCooldown)
                return false;

            StartCoroutine(ActivateCooldown());

            return true;
        }

        /// <summary>
        /// A use function that is called from <see cref="Interactable"/> to <see cref="Interactable"/>.
        /// </summary>
        /// <param name="useableGrabbable">The origin <see cref="Interactable"/>.</param>
        /// <returns>The status of if the Interaction was successfull.</returns>
        public virtual bool Use(UseableGrabbable useableGrabbable) => false;

        private protected virtual IEnumerator ActivateCooldown()
        {
            IsOnCooldown = true;
            yield return new WaitForSeconds(_cooldown);
            IsOnCooldown = false;
        }
    }
}