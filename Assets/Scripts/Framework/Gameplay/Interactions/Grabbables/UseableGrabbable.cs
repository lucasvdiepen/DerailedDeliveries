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

            Interactable targetInteractable = GetCollidingInteractable(interactor);
            if(targetInteractable != null && targetInteractable.Interact(this))
                return;

            base.UseGrabbable(interactor);
        }

        [Server]
        private protected virtual Interactable GetCollidingInteractable(Interactor interactor)
        {
            Collider[] colliders = Physics.OverlapBox(BoxCollider.center + transform.position, BoxCollider.size);

            foreach(Collider collider in colliders)
            {
                if(!collider.TryGetComponent(out Interactable interactable))
                    continue;

                if(CheckCollidingType(interactable) && interactable.CheckIfInteractable(interactor))
                    return interactable;
            }

            return null;
        }

        private protected abstract bool CheckCollidingType(Interactable interactable);
    }
}