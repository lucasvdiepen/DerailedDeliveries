using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.CoalOvenSystem;
using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    /// <summary>
    /// An <see cref="Interactable"/> responsible for handling the coal oven.
    /// </summary>
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="useableGrabbable"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
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