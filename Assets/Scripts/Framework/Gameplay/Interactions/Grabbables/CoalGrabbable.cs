using DerailedDeliveries.Framework.Gameplay.Interactions.Interactables;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="UseableGrabbable"/> class responsible for handling the coal.
    /// </summary>
    public class CoalGrabbable : UseableGrabbable
    {
        private protected override bool CheckCollidingType(Interactable interactable)
            => interactable is CoalOvenInteractable;
    }
}