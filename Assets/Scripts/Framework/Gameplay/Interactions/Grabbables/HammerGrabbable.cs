using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Interactions.Interactables;
using DerailedDeliveries.Framework.DamageRepairManagement;
using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="UseableGrabbable"/> class that handles logic for the hammer.
    /// </summary>
    public class HammerGrabbable : UseableGrabbable
    {
        private protected override bool CheckCollidingType(Interactable interactable)
            => interactable is IRepairable || interactable is ShelfInteractable;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfUseable(Interactor interactor)
            => IsInteractable 
            && !IsOnCooldown 
            && interactor.InteractingTarget == this 
            && GetRepairableInteractable(GetCollidingColliders()) != null;

        [Server]
        private protected override Interactable GetCollidingInteractable(Interactor interactor, bool isUse)
        {
            if(!isUse)
                return base.GetCollidingInteractable(interactor, isUse);

            Collider[] colliders = GetCollidingColliders();

            Interactable interactable = GetRepairableInteractable(colliders);

            if(interactable != null)
                return interactable;

            foreach(Collider collider in colliders)
            {
                if(!collider.TryGetComponent(out interactable))
                    continue;

                if(!CheckCollidingType(interactable))
                    continue;

                if(!interactable.CheckIfInteractable(interactor))
                    continue;

                return interactable;
            }

            return null;
        }

        private Interactable GetRepairableInteractable(Collider[] colliders)
        {
            foreach (Collider collider in colliders)
            {
                if (!collider.TryGetComponent(out Interactable interactable))
                    continue;

                if (!CheckCollidingType(interactable))
                    continue;

                IRepairable repairable = (IRepairable)interactable;
                if (!repairable.CanBeRepaired())
                    continue;

                return interactable;
            }

            return null;
        }

        [Server]
        private protected override bool RunUse(Interactable interactable)
        {
            IRepairable repairable = (IRepairable)interactable;
            if(repairable.CanBeRepaired())
            {
                repairable.Repair();
                return true;
            }

            return base.RunUse(interactable);
        }
    }
}