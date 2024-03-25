using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A Interactable class that is used for all grabbable Interactables.
    /// </summary>
    public class Grabbable : Interactable
    {
        private Interactor _originInteractor;

        private protected bool IsBeingInteracted { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CheckIfInteractable() => IsInteractable && !IsOnCooldown && !IsBeingInteracted;

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
