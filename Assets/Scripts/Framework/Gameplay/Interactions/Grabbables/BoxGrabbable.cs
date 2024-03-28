using DerailedDeliveries.Framework.Gameplay.Interactions.InteractTargets;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Grabbable"/> class that is responsible for holding logic for the Box Grabbable.
    /// </summary>
    public class BoxGrabbable : Grabbable
    {
        private protected override Interactable GetInteractableTarget()
        {
            foreach(Interactable interactable in CollidingInteractables)
            {
                if (interactable.GetType() == typeof(ShelfInteractable))
                    return interactable;
            }

            return null;
        }
    }
}
