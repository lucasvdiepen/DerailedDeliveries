using FishNet.Object.Synchronizing;
using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Interactions.InteractTargets;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Interactable"/> class that is used for all grabbable Interactables.
    /// </summary>
    public class Grabbable : Interactable
    {
        [SerializeField]
        private Collider[] _collidingInteractables;

        [SerializeField]
        private float _groundCheckDistance = 5f;

        [SerializeField]
        private BoxCollider _boxCollider;

        [SerializeField]
        private Interactor _originInteractor;

        /// <summary>
        /// Returns the <see cref="Interactor"/> of this Interactable, will be null if not being 
        /// targeted/interacted with.
        /// </summary>
        public Interactor OriginInteractor => _originInteractor;

        /// <summary>
        /// A getter that returns this interactable's <see cref="BoxCollider"/>.
        /// </summary>
        public BoxCollider BoxCollider => _boxCollider;

        /// <summary>
        /// A getter that returns the CollidingInteractables list.
        /// </summary>
        public Collider[] CollidingInteractables { get; set; }

        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private protected bool IsBeingInteracted { get; set; }

        private protected virtual void Awake()
        {
            if (_boxCollider == null)
                _boxCollider = GetComponent<BoxCollider>();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CheckIfInteractable() => base.CheckIfInteractable() && !IsBeingInteracted;

        [Server]
        private protected override bool Interact(Interactor interactor)
        {
            if (!base.Interact(interactor) || IsBeingInteracted && interactor != _originInteractor)
                return false;

            CheckPickupAndUsage(interactor);

            return true;
        }

        [Server]
        private protected virtual void CheckPickupAndUsage(Interactor interactor)
        {
            if (!IsBeingInteracted)
            {
                NetworkObject.SetParent(interactor.GrabbingAnchor.GetComponent<NetworkBehaviour>());
                transform.localPosition = Vector3.zero;

                UpdateInteractionStatus(interactor, true);
                interactor.UpdateInteractingTarget(this, IsBeingInteracted);
                return;
            }

            Interactable target = GetInteractableTarget();
            if (target != null && target.InteractableInteracts(this))
                return;

            NetworkObject.UnsetParent();
            UpdateInteractionStatus(null, false);

            interactor.UpdateInteractingTarget(null, IsBeingInteracted);
            PlaceInteractableOnGround();
            return;
        }

        public void UpdateInteractionStatus(Interactor interactor, bool isBeingInteracted)
        {
            IsBeingInteracted = isBeingInteracted;
            _originInteractor = interactor;
        }

        private protected virtual Interactable GetInteractableTarget() => null;

        private protected virtual void PlaceInteractableOnGround()
        {
            if (!gameObject.TryGetComponent(out BoxCollider collider))
                return;

            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _groundCheckDistance);

            hit.point += new Vector3(0, collider.size.y * .5f, 0);
            transform.position = hit.point;
        }
    }
}
