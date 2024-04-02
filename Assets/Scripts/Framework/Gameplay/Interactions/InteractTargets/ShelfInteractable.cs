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

                if(interactable is Grabbable grabbable)
                    grabbable.OriginInteractor.UpdateInteractingTarget(null, false);

                _heldInteractable = interactable;
                return true;
            }
            else if (true)
            {
                // If there is not _heldInteractable the only way they can use something here is by it being a repair
                // tool.
            }

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
            return targetInteractable.InteractServer(interactor);
        }
    }
}