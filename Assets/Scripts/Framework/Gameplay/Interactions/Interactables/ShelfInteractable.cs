using FishNet.Object.Synchronizing;
using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    /// <summary>
    /// A <see cref="Interactable"/> class that handles logic for the Shelf interactable.
    /// </summary>
    public class ShelfInteractable : Interactable
    {
        [SerializeField, SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private UseableGrabbable _heldGrabbable;

        [SerializeField]
        private NetworkBehaviour _grabbableAnchor;

        /// <summary>
        /// A getter that returns the <see cref="UseableGrabbable"/> that this <see cref="ShelfInteractable"/> is 
        /// holding.
        /// </summary>
        public UseableGrabbable HeldGrabbable => _heldGrabbable;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfInteractable(Interactor interactor)
        {
            return base.CheckIfInteractable(interactor) 
                && (_heldGrabbable == null ^ interactor.InteractingTarget == null);
        }

        [Server]
        private protected override bool Interact(Interactor interactor)
        {
            if (!base.Interact(interactor))
                return false;

            return GrabFromShelf(interactor);
        }

        /// <summary>
        /// A function that calls an Interact from <see cref="Interactable"/> to this <see cref="Interactable"/>.
        /// </summary>
        /// <param name="interactable">The origin <see cref="Interactable"/>.</param>
        /// <returns>The result of if the Interact was succesfull.</returns>
        [Server]
        public override bool Interact(UseableGrabbable useableGrabbable)
        {
            // Can add else statement here for a check if the Interactable is a repair item
            if (_heldGrabbable != null)
                return false;

            _heldGrabbable = useableGrabbable;

            _heldGrabbable.NetworkObject.SetParent(_grabbableAnchor);
            _heldGrabbable.transform.localPosition = Vector3.zero;
            _heldGrabbable.PlaceOnGround();

            _heldGrabbable.OriginInteractor.UpdateInteractingTarget(_heldGrabbable.OriginInteractor.Owner, null, false);
            return true;
        }

        [Server]
        private bool GrabFromShelf(Interactor interactor)
        {
            if (_heldGrabbable == null)
                return false;

            _heldGrabbable.UpdateInteractionStatus(null, false);

            UseableGrabbable targetInteractable = _heldGrabbable;
            _heldGrabbable = null;

            interactor.UpdateInteractingTarget(interactor.Owner, targetInteractable, true);
            return targetInteractable.InteractAsServer(interactor);
        }
    }
}