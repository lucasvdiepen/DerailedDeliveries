using DerailedDeliveries.Framework.DamageRepairManagement;
using DerailedDeliveries.Framework.Gameplay.Player;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    public class HammerGrabbable : UseableGrabbable
    {
        private protected override bool CheckCollidingType(Interactable interactable)
            => interactable.GetComponent<IRepairable>() != null;

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

                IRepairable[] repairables = interactable.GetComponents<IRepairable>();
                foreach(IRepairable repairable in repairables)
                {
                    if (!repairable.CanBeRepaired())
                        continue;

                    return interactable;
                }
            }

            return null;
        }

        [Server]
        private protected override bool RunInteract(Interactable interactable)
        {
            IRepairable[] repairables = interactable.GetComponents<IRepairable>();

            foreach (IRepairable repairable in repairables)
            {
                if(!repairable.CanBeRepaired())
                    continue;

                repairable.Repair();
            }

            return true;
        }
    }
}