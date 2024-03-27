using FishNet.Object.Synchronizing;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using FishNet.Object;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A Interactable class that is used for all grabbable Interactables.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class Grabbable : Interactable
    {
        private Interactor _originInteractor;

        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private protected bool IsBeingInteracted { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CheckIfInteractable() => base.CheckIfInteractable() && !IsBeingInteracted;

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
                gameObject.transform.localPosition = Vector3.zero;
            }
            else
            {
                NetworkObject.UnsetParent();

                if (!gameObject.TryGetComponent(out BoxCollider collider))
                    return;

                Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit hit, 5f);

                hit.point += new Vector3(0, collider.size.y * .5f, 0);
                gameObject.transform.position = hit.point;
            }
        }
    }
}
