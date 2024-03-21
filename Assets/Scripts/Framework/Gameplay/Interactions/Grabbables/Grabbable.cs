using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A Interactable class that is used for all Interactables that are grabbable.
    /// </summary>
    public class Grabbable : Interactable
    {
        /// <summary>
        /// A function that handles interacting with this Grabbable.
        /// </summary>
        /// <param name="interactor">The Interactor that interacts with this Grabbable.</param>
        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);

            interactor.SetInteractingTarget(this);
        }
    }
}
