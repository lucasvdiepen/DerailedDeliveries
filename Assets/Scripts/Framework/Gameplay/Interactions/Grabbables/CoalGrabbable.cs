using DerailedDeliveries.Framework.Gameplay.Interactions.Interactables;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    public class CoalGrabbable : UseableGrabbable
    {
        private protected override bool CheckCollidingType(Interactable interactable)
            => interactable is CoalOvenInteractable;
    }
}