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
        private protected override void UseGrabbable(Interactor interactor)
        {
            if(!IsBeingInteracted)
            {
                base.UseGrabbable(interactor);
                return;
            }

            Interactable targetInteractable = GetCollidingInteractable(interactor, false);
            if(targetInteractable != null && RunInteract(targetInteractable))
                return;

            if(IsDeinitializing)
                return;

            base.UseGrabbable(interactor);
        }

        [Server]
        private protected override bool Use(Interactor interactor)
        {
            if (!IsBeingInteracted)
                return false;

            if (!base.Use(interactor))
                return false;

            Interactable targetInteractable = GetCollidingInteractable(interactor, true);
            if (targetInteractable != null && RunUse(targetInteractable))
                return false;

            return true;
        }

        [Server]
        private protected virtual Interactable GetCollidingInteractable(Interactor interactor, bool isUse)
        {
            Collider[] colliders = GetCollidingColliders();

            foreach(Collider collider in colliders)
            {
                if(!collider.TryGetComponent(out Interactable interactable))
                    continue;

                if(!CheckCollidingType(interactable))
                    continue;

                if(isUse && !interactable.CheckIfUseable(interactor))
                    continue;

                if(!isUse && !interactable.CheckIfInteractable(interactor))
                    continue;

                return interactable;
            }

            return null;
        }

        [Server]
        private protected virtual bool RunInteract(Interactable interactable) => interactable.Interact(this);

        [Server]
        private protected virtual bool RunUse(Interactable interactable) => interactable.Use(this);

        private protected abstract bool CheckCollidingType(Interactable interactable);
    }
}