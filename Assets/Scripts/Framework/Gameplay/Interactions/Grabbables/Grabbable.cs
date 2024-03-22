using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A Interactable class that is used for all Interactables that are grabbable.
    /// </summary>
    public class Grabbable : Interactable
    {
        private Interactor _originInteractor;

        /// <summary>
        /// A function that handles interacting with this Grabbable.
        /// </summary>
        /// <param name="interactor">The Interactor that interacts with this Grabbable.</param>
        private protected override void Interact(Interactor interactor)
        {
            base.Interact(interactor);

            if (IsBeingInteracted && interactor != _originInteractor)
                return;

            IsBeingInteracted = !IsBeingInteracted;

            _originInteractor = IsBeingInteracted
                ? interactor
                : null;

            interactor.SetInteractingTarget(this, IsBeingInteracted);
        }
    }
}
