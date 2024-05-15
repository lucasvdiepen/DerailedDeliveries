using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.DamageRepairManagement;
using DerailedDeliveries.Framework.Gameplay.Interactions.Interactables;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="UseableGrabbable"/> class that handles logic for the hammer.
    /// </summary>
    public class HammerGrabbable : UseableGrabbable
    {
        private protected override bool CheckCollidingType(Interactable interactable)
            => interactable is IRepairable || interactable is ShelfInteractable;

        [Server]
        private protected override Interactable GetCollidingInteractable(Interactor interactor)
        {
            Collider[] colliders = Physics.OverlapBox(BoxCollider.center + transform.position, BoxCollider.size);

            foreach (Collider collider in colliders)
            {
                if (!collider.TryGetComponent(out Interactable interactable))
                    continue;

                if (!CheckCollidingType(interactable))
                    continue;

                IRepairable repairable = (IRepairable)interactable;
                if(!repairable.CanBeRepaired())
                    continue;

                return interactable;
            }

            foreach(Collider collider in colliders)
            {
                if(!collider.TryGetComponent(out Interactable interactable))
                    continue;

                if(!CheckCollidingType(interactable))
                    continue;

                if(!interactable.CheckIfInteractable(interactor))
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