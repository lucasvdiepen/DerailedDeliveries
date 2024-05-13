using UnityEngine;
using FishNet.Object;

using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Grabbable"/> class that is able to be used on a <see cref="Interactable"/>.
    /// </summary>
    public abstract class UseableGrabbable : Grabbable
    {
        [Server]
        private protected override bool GrabGrabbable(Interactor interactor)
        {
            if(!IsBeingInteracted)
            {
                base.GrabGrabbable(interactor);
                return true;
            }

            Interactable targetInteractable = GetCollidingInteractable(interactor);
            if(targetInteractable != null && RunInteract(targetInteractable))
                return true;

            if(IsDeinitializing)
                return false;

            base.GrabGrabbable(interactor);
            return true;
        }

        [Server]
        private protected virtual Interactable GetCollidingInteractable(Interactor interactor)
        {
            Collider[] colliders = GetCollidingColliders();

            foreach(Collider collider in colliders)
            {
                if(!collider.TryGetComponent(out Interactable interactable))
                    continue;

                if(!CheckCollidingType(interactable))
                    continue;

                if(!interactable.CheckIfInteractable(interactor))
                    continue;

                return interactable;
            }

            return null;
        }

        [Server]
        private protected virtual bool RunInteract(Interactable interactable) => interactable.Interact(this);

        private protected abstract bool CheckCollidingType(Interactable interactable);
    }
}