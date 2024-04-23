using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    public class TrainSpeedButtonInteractable : Interactable
    {
        [SerializeField]
        private bool _isForwardButton;

        private protected override bool Interact(Interactor interactor)
        {
            if(!base.Interact(interactor))
                return false;

            TrainEngine.Instance.AdjustSpeed(_isForwardButton);
            return true;
        }
    }
}