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

        private protected override bool RunInteract(Interactable interactable)
        {
            IRepairable damageable = interactable.GetComponent<IRepairable>();
            damageable.Repair();
            return true;
        }
    }
}