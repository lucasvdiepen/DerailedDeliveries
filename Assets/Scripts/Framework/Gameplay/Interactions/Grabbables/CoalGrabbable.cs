using DerailedDeliveries.Framework.Gameplay.Interactions.Interactables;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// An <see cref="UseableGrabbable"/> responsible for handling the coal.
    /// </summary>
    public class CoalGrabbable : UseableGrabbable
    {
        private protected override bool CheckCollidingType(Interactable interactable)
            => interactable is CoalOvenInteractable;
    }
}