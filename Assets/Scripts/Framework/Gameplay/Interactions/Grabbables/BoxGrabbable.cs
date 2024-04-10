using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Interactions.Interactables;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Grabbable"/> class that is responsible for holding logic for the Box <see cref="Grabbable"/>.
    /// </summary>
    public class BoxGrabbable : UseableGrabbable
    {
        private protected override Interactable GetCollidingInteractable(Interactor interactor)
        {
            Collider[] colliders = GetCollidingColliders();

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out ShelfInteractable interactable))
                    return interactable;

                else if (collider.TryGetComponent(out DeliveryBelt belt))
                    return belt;
            }

            return null;
        }

        private protected override bool CheckCollidingType(Interactable interactable)
            => interactable.GetType() == typeof(ShelfInteractable);
    }
}
