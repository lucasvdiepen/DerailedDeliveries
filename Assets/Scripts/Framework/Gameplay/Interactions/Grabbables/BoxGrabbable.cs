using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Interactions.InteractTargets;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Grabbable"/> class that is responsible for holding logic for the Box <see cref="Grabbable"/>.
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
                if (!collider.TryGetComponent(out ShelfInteractable shelfInteractable))
                    continue;

                if (!CheckCollidingType(shelfInteractable))
                    continue;

                if (interactor.InteractingTarget != null && shelfInteractable.HeldGrabbable != null ||
                    interactor.InteractingTarget == null && shelfInteractable.HeldGrabbable == null)
                    continue;

                return shelfInteractable;
            }

            return null;
        }
    }
}
