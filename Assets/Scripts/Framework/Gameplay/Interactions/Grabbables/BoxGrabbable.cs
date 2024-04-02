using DerailedDeliveries.Framework.Gameplay.Interactions.InteractTargets;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Grabbable"/> class that is responsible for holding logic for the Box Grabbable.
    /// </summary>
    public class BoxGrabbable : Grabbable
    {
        private protected override Interactable GetInteractableTarget()
        {
            CollidingInteractables = Physics.OverlapBox(BoxCollider.center + transform.position, BoxCollider.size * .5f);

            foreach (Collider collider in CollidingInteractables)
            {
                if (!collider.TryGetComponent(out Interactable interactable))
                    continue;

                if (interactable.GetType() == typeof(ShelfInteractable))
                    return interactable;
            }

            return null;
        }
    }
}
