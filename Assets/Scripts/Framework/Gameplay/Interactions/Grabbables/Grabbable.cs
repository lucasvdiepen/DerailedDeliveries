using FishNet.Object.Synchronizing;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A Interactable class that is used for all grabbable Interactables.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class Grabbable : Interactable
    {
        private Interactor _originInteractor;

        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private protected bool IsBeingInteracted { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CheckIfInteractable() => base.CheckIfInteractable() && !IsBeingInteracted;

        private protected override bool Interact(Interactor interactor)
        {
            if (!base.Interact(interactor) || IsBeingInteracted && interactor != _originInteractor)
                return false;

            IsBeingInteracted = !IsBeingInteracted;

            _originInteractor = IsBeingInteracted
                ? interactor
                : null;

            interactor.SetInteractingTarget(this, IsBeingInteracted);

            return true;
        }
    }
}
