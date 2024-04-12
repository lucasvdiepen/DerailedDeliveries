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
        [SerializeField]
        private int _packageID = -1;

        [SerializeField]
        private string _packageLabel;

        [SerializeField]
        private TextMeshProUGUI[] _textDisplays;

        /// <summary>
        /// A getter that is used to return the package's ID.
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

            for(int i = 0; i < _textDisplays.Length; i++)
                _textDisplays[i].text = _packageLabel;
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
