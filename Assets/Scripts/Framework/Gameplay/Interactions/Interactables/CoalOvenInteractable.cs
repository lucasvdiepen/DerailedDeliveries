using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.CoalOvenSystem;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    public class CoalOvenInteractable : Interactable
    {
        [SerializeField]
        private int _coalToAdd = 10;

        [Server]
        private protected override bool Interact(Interactor interactor)
        {
            if(!base.Interact(interactor))
                return false;

            CoalOven.Instance.EnableOven();

            return true;
        }

        [Server]
        public override bool Interact(UseableGrabbable useableGrabbable)
        {
            CoalOven.Instance.AddCoal(_coalToAdd);

            useableGrabbable.OriginInteractor.UpdateInteractingTarget(useableGrabbable.OriginInteractor.Owner, null, false);
            useableGrabbable.Despawn();

            return base.Interact(useableGrabbable);
        }
    }
}