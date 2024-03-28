using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Player;

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

            return InteractWithShelfItems(interactor);
        }

        private bool InteractWithShelfItems(Interactor interactor)
        {
            Interactable target = interactor.InteractingTarget;

            if (_heldInteractable == null)
            {
                interactor.InteractingTarget.NetworkObject.SetParent(_interactableAnchor);
                _heldInteractable = target;

                if (target is Grabbable grabbable)
                    grabbable.UpdateInteractionStatus(interactor, false);

                interactor.UpdateInteractingTarget(null, false);
                return true;
            }

            if (_heldInteractable != null && target == null)
            {
                _heldInteractable.NetworkObject.SetParent(interactor.GrabbingAnchor);
                _heldInteractable.transform.localPosition = Vector3.zero;

                if (target is Grabbable grabbable)
                    grabbable.UpdateInteractionStatus(interactor, true);

                interactor.UpdateInteractingTarget(_heldInteractable, true);

                _heldInteractable = target;
                return true;
            }

            return false;
        }
    }
}