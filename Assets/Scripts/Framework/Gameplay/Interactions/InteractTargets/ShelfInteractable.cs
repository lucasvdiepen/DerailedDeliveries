using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DG.Tweening;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.InteractTargets
{
    /// <summary>
    /// A <see cref="Interactable"/> class that handles logic for the Shelf interactable.
    /// </summary>
    public class ShelfInteractable : Interactable
    {
        [SerializeField]
        private Interactable _heldInteractable;

        [SerializeField]
        private NetworkBehaviour _interactableAnchor;

        private protected override bool Interact(Interactor interactor)
        {
            if (!base.Interact(interactor))
                return false;

            return GrabFromShelf(interactor);
        }

        public override bool InteractableInteracts(Interactable interactable)
        {
            if (_heldInteractable == null)
            {
                interactable.NetworkObject.SetParent(_interactableAnchor);
                interactable.transform.localPosition = Vector3.zero;

                if (interactable is Grabbable grabbable)
                {
                    grabbable.OriginInteractor.UpdateInteractingTarget(null, false);
                    grabbable.PlaceOnGround();
                }

                _heldInteractable = interactable;
                return true;
            }
            
            // Can add else statement here for a check if the Interactable is a repair item

            return false;
        }

        private bool GrabFromShelf(Interactor interactor)
        {
            if (_heldInteractable == null)
                return false;

            if (_heldInteractable is Grabbable grabbable)
                grabbable.UpdateInteractionStatus(null, false);

            Interactable targetInteractable = _heldInteractable;

            _heldInteractable = null;
            interactor.UpdateInteractingTargetClient(interactor.Owner, targetInteractable, true);
            return targetInteractable.InteractServer(interactor);
        }
    }
}