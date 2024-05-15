using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    /// <summary>
    /// An <see cref="Interactable"/> class that handles logic for the train direction lever interactable.
    /// </summary>
    public class TrainDirectionLeverInteractable : Interactable
    {
        private protected override bool Use(Interactor interactor)
        {
            if(!base.Use(interactor))
                return false;

            TrainEngine.Instance.ToggleTrainDirection();
            return true;
        }
    }
}