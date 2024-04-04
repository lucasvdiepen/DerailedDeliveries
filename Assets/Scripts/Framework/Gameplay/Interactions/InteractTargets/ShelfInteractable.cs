using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.InteractTargets
{
    /// <summary>
    /// A <see cref="Interactable"/> class that handles logic for the Shelf interactable.
    /// </summary>
    public class ShelfInteractable : Interactable
    {
        [SerializeField]
        private Interactable _heldGrabbable;

        [SerializeField]
        private NetworkBehaviour _grabbableAnchor;

        public override bool CheckIfInteractable() => base.CheckIfInteractable();

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
        public override bool Interact(Interactable interactable)
        {
            // Can add else statement here for a check if the Interactable is a repair item
            if (_heldGrabbable != null)
                return false;

            if (!(interactable is Grabbable grabbable))
                return false;

            grabbable.NetworkObject.SetParent(_grabbableAnchor);
            grabbable.transform.localPosition = Vector3.zero;
            grabbable.PlaceOnGround();

            grabbable.OriginInteractor.UpdateInteractingTarget(grabbable.OriginInteractor.Owner, null, false);

            _heldGrabbable = grabbable;
            return true;
        }

        private bool GrabFromShelf(Interactor interactor)
        {
            if (_heldGrabbable == null)
                return false;

            if (_heldGrabbable is Grabbable grabbable)
                grabbable.UpdateInteractionStatus(null, false);

            Interactable targetInteractable = _heldGrabbable;

            _heldGrabbable = null;
            interactor.UpdateInteractingTarget(interactor.Owner, targetInteractable, true);
            return targetInteractable.InteractAsServer(interactor);
        }
    }
}