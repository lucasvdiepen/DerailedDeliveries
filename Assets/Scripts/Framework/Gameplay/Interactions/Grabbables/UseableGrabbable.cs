using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Grabbable"/> class that is able to be used on a <see cref="Interactable"/>.
    /// </summary>
    public abstract class UseableGrabbable : Grabbable
    {
        private protected override void UseGrabbable(Interactor interactor)
        {
            if(!IsBeingInteracted)
            {
                base.UseGrabbable(interactor);
                return;
            }

            Interactable targetInteractable = GetInteractable(interactor);
            if(targetInteractable != null && targetInteractable.Interact(this))
                return;

            base.UseGrabbable(interactor);
        }

        private protected virtual Interactable GetInteractable(Interactor interactor)
        {
            Collider[] colliders = Physics.OverlapBox(BoxCollider.center + transform.position, BoxCollider.size);

            foreach(Collider collider in colliders)
            {
                if(!collider.TryGetComponent(out Interactable interactable))
                    continue;

                if(CheckCollidingType(interactable) && interactable.CheckIfInteractable())
                    return interactable;
            }

            return null;
        }

        private protected abstract bool CheckCollidingType(Interactable interactable);
    }
}