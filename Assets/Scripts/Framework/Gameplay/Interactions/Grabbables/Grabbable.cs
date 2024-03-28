using FishNet.Object.Synchronizing;
using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A Interactable class that is used for all grabbable Interactables.
    /// </summary>
    public class Grabbable : Interactable
    {
        [SerializeField]
        private protected float _groundCheckDistance = 5f;

        private Interactor _originInteractor;

        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private protected bool IsBeingInteracted { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CheckIfInteractable() => base.CheckIfInteractable() && !IsBeingInteracted;

        [Server]
        private protected override bool Interact(Interactor interactor)
        {
            if (!base.Interact(interactor) || IsBeingInteracted && interactor != _originInteractor)
                return false;

            IsBeingInteracted = !IsBeingInteracted;

            _originInteractor = IsBeingInteracted
                ? interactor
                : null;

            UseGrabbable(interactor);

            return true;
        }

        [Server]
        private protected virtual void UseGrabbable(Interactor interactor)
        {
            interactor.UpdateInteractingTarget(this, IsBeingInteracted);

            if (IsBeingInteracted)
            {
                NetworkObject.SetParent(interactor.GrabbingAnchor.GetComponent<NetworkBehaviour>());
                transform.localPosition = Vector3.zero;
            }
            else
            {
                NetworkObject.UnsetParent();

                if (!gameObject.TryGetComponent(out BoxCollider collider))
                    return;

                Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _groundCheckDistance);

                hit.point += new Vector3(0, collider.size.y * .5f, 0);
                transform.position = hit.point;
            }
        }
    }
}
