using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    public abstract class UseableGrabbable : Grabbable
    {
        private protected override void UseGrabbable(Interactor interactor)
        {
            if(!IsBeingInteracted)
            {
                base.UseGrabbable(interactor);
                return;
            }

            Interactable targetInteractable = GetInteractable();
            if(targetInteractable != null && targetInteractable.Interact(this))
                return;

            base.UseGrabbable(interactor);
        }

        private Interactable GetInteractable()
        {
            Collider[] colliders = Physics.OverlapBox(BoxCollider.center + transform.position, BoxCollider.size * .5f);

            foreach(Collider collider in colliders)
            {
                if(!collider.TryGetComponent(out Interactable interactable))
                    continue;

                if(CheckCollidingType(interactable))
                    return interactable;
            }

            return null;
        }

        private protected abstract bool CheckCollidingType(Interactable interactable);
    }
}