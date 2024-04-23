using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
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