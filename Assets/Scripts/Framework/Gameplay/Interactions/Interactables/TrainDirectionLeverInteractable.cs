using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    /// <summary>
    /// An <see cref="Interactable"/> class that handles logic for the train direction lever interactable.
    /// </summary>
    public class TrainDirectionLeverInteractable : Interactable
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfUseable(Interactor interactor)
            => IsInteractable && !IsOnCooldown && interactor.InteractingTarget == null;

        private protected override bool Use(Interactor interactor)
        {
            if(!base.Use(interactor))
                return false;

            TrainEngine.Instance.ToggleTrainDirection();
            return true;
        }
    }
}