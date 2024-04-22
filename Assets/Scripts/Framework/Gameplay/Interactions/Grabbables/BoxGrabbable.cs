using UnityEngine;
using TMPro;

using DerailedDeliveries.Framework.Gameplay.Interactions.Interactables;
using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Grabbable"/> class that is responsible for holding logic for the Box <see cref="Grabbable"/>.
    /// </summary>
    public class BoxGrabbable : UseableGrabbable
    {
        private protected override bool CheckCollidingType(Interactable interactable)
            => interactable.GetType() == typeof(ShelfInteractable) 
            || interactable.GetType() == typeof(DeliveryBeltInteractable);
    }
}
