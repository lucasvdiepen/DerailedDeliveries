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
            if (!IsInteractable || IsOnCooldown)
                return;

            if (IsBeingInteracted && interactor != _originInteractor)
                return;

            base.Interact(interactor);

            IsBeingInteracted = !IsBeingInteracted;

            _originInteractor = IsBeingInteracted
                ? interactor
                : null;

            interactor.SetInteractingTarget(this, IsBeingInteracted);
        }
    }
}
