using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    /// <summary>
    /// A <see cref="Interactable"/> class that handles logic for the train direction lever interactable.
    /// </summary>
    public class TrainDirectionLeverInteractable : Interactable
    {
        private protected override bool Interact(Interactor interactor)
        {
            if(!base.Interact(interactor))
                return false;

            TrainEngine.Instance.ToggleTrainDirection();
            return true;
        }
    }
}