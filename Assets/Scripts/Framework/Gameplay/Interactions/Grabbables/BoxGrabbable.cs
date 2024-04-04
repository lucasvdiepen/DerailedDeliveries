using DerailedDeliveries.Framework.Gameplay.Interactions.InteractTargets;
using DerailedDeliveries.Framework.Gameplay.Player;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Grabbable"/> class that is responsible for holding logic for the Box Grabbable.
    /// </summary>
    public class BoxGrabbable : UseableGrabbable
    {
        private protected override bool CheckCollidingType(Interactable interactable)
            => interactable.GetType() == typeof(ShelfInteractable);

        private protected override Interactable GetInteractable(Interactor interactor)
        {
            Collider[] colliders = Physics.OverlapBox(BoxCollider.center + transform.position, BoxCollider.size);

            foreach(Collider collider in colliders)
            {
                if(!collider.TryGetComponent(out Interactable interactable))
                    continue;

                if(CheckCollidingType(interactable) && (interactable is ShelfInteractable shelfInteractable))
                {
                    if(interactor.InteractingTarget != null && shelfInteractable.HeldGrabbable != null)
                        continue;

                    if(interactor.InteractingTarget == null && shelfInteractable.HeldGrabbable == null)
                        continue;

                    return interactable;
                }
            }

            return null;
        }
    }
}
