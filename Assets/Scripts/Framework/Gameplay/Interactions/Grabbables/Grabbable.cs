using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Interactions.InteractTargets;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Interactable"/> class that is used for all grabbable Interactables.
    /// </summary>
    public abstract class Grabbable : Interactable
    {
        [SerializeField]
        private List<Interactable> _collidingInteractables = new();

        [SerializeField]
        private Interactor _originInteractor;

        [SerializeField]
        private ShelfInteractable _originShelf;

        [SerializeField]
        private float _groundCheckDistance = 5f;

        /// <summary>
        /// A getter that returns the CollidingInteractables list.
        /// </summary>
        public List<Interactable> CollidingInteractables => _collidingInteractables;

        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private protected bool IsBeingInteracted { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (IsBeingInteracted && other.TryGetComponent(out Interactable interactable))
                _collidingInteractables.Add(interactable);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Interactable interactable) && _collidingInteractables.Contains(interactable))
                _collidingInteractables.Remove(interactable);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CheckIfInteractable() => base.CheckIfInteractable() && !IsBeingInteracted;

        [Server]
        private protected override bool Interact(Interactor interactor)
        {
            if (!base.Interact(interactor) || IsBeingInteracted && interactor != _originInteractor)
                return false;

            PickupGrabbable(interactor);

            return true;
        }

        [Server]
        private protected virtual void PickupGrabbable(Interactor interactor)
        {
            if (UseGrabbable(interactor))
                return;

            UpdateInteractionStatus(interactor, null, !IsBeingInteracted);

            if (IsBeingInteracted)
            {
                NetworkObject.SetParent(interactor.GrabbingAnchor.GetComponent<NetworkBehaviour>());
                transform.localPosition = Vector3.zero;
                interactor.UpdateInteractingTarget(this, IsBeingInteracted);
            }
            else
            {
                NetworkObject.UnsetParent();

                interactor.UpdateInteractingTarget(null, IsBeingInteracted);
                PlaceInteractableOnGround();
            }
        }

        private protected virtual void PlaceInteractableOnGround()
        {
            if (!gameObject.TryGetComponent(out BoxCollider collider))
                return;

            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _groundCheckDistance);

            hit.point += new Vector3(0, collider.size.y * .5f, 0);
            transform.position = hit.point;
        }

        /// <summary>
        /// Updates the Grabbable's Interaction status.
        /// </summary>
        /// <param name="interactor">The Interactor that's updating the status.</param>
        /// <param name="isBeingInteracted">The new IsBeingInteracted status.</param>
        public void UpdateInteractionStatus(Interactor interactor, ShelfInteractable shelf, bool isBeingInteracted)
        {
            _originShelf = shelf;
            IsBeingInteracted = isBeingInteracted;
            _originInteractor = IsBeingInteracted
                ? interactor
                : null;
        }

        private protected virtual bool UseGrabbable(Interactor interactor)
        {
            if (_originShelf == null && _collidingInteractables.Count == 0)
                return false;

            Interactable target = GetInteractableTarget();

            if (target == null)
                return false;

            return target.InteractableInteracts(interactor);
        }

        private protected abstract Interactable GetInteractableTarget();
    }
}
