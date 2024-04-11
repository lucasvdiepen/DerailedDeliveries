using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Interactions.Interactables;
using System.Reflection.Emit;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Grabbable"/> class that is responsible for holding logic for the Box <see cref="Grabbable"/>.
    /// </summary>
    public class BoxGrabbable : UseableGrabbable
    {
        [SerializeField]
        private int _packageID = -1;

        [SerializeField]
        private string _packageLabel;

        [SerializeField]
        private int _deliveryQuality = 10;

        /// <summary>
        /// A getter to get the Package's quality.
        /// </summary>
        public int PackageQuality => _deliveryQuality;

        /// <summary>
        /// 
        /// </summary>
        public int PackageID => _packageID;

        /// <summary>
        /// A function that updates the packageLabel and packageID.
        /// </summary>
        /// <param name="label">The new package label.</param>
        /// <param name="id">The new package ID.</param>
        public void UpdateLabelAndID(string label, int id)
        {
            _packageLabel = label;
            _packageID = id;
        }

        private protected override Interactable GetCollidingInteractable(Interactor interactor)
        {
            Collider[] colliders = GetCollidingColliders();

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out ShelfInteractable interactable))
                    return interactable;

                else if (collider.TryGetComponent(out DeliveryBelt belt))
                    return belt;
            }

            return null;
        }

        private protected override bool CheckCollidingType(Interactable interactable)
            => interactable.GetType() == typeof(ShelfInteractable);
    }
}
