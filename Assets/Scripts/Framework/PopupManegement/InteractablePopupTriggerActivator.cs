using UnityEngine;
using UnityEngine.UI;

using DerailedDeliveries.Framework.Gameplay.Interactions;
using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.TriggerArea;

namespace DerailedDeliveries.Framework.PopupManagement
{
    /// <summary>
    /// A class that is responsible for showing and hiding a popup when colliding with an interactable.
    /// </summary>
    [RequireComponent(typeof(Interactor))]
    public class InteractablePopupTriggerActivator : NetworkTriggerAreaBase<Interactable>
    {
        [Header("TriggerArea collider")]
        [SerializeField]
        private SphereCollider _collider;

        [SerializeField]
        private Popup popup;

        [SerializeField]
        private Image _popupImage;

        [SerializeField]
        private Sprite _popupInteractSprite;

        [SerializeField]
        private Sprite _popupUseSprite;

        private Interactor _interactor;

        private protected virtual void Awake()
        {
            _interactor = GetComponent<Interactor>();

            if (_collider == null)
                _collider = GetComponent<SphereCollider>();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            TimeManager.OnPostTick += CheckInteractables;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStopClient()
        {
            base.OnStopClient();

            TimeManager.OnPostTick -= CheckInteractables;
        }

        private protected override Collider[] GetCollidingColliders()
            => Physics.OverlapSphere((transform.rotation * _collider.center) + transform.position, _collider.radius);

        private void CheckInteractables()
        {
            Interactable[] interactables = ComponentsInCollider;

            // Check if any interactable is useable.
            foreach (Interactable interactable in interactables)
            {
                if(interactable.CheckIfUseable(_interactor))
                {
                    _popupImage.sprite = _popupUseSprite;
                    popup.Show();

                    return;
                }
            }

            // Check if any interactable is interactable.
            foreach (Interactable interactable in interactables)
            {
                if(interactable.CheckIfInteractable(_interactor))
                {
                    _popupImage.sprite = _popupInteractSprite;
                    popup.Show();

                    return;
                }
            }

            // Close the popup if no suitable interactable is found.
            popup.Close();
        }
    }
}