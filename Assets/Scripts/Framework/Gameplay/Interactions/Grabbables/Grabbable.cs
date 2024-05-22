using FishNet.Object.Synchronizing;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Utils.ObjectParenting;
using DerailedDeliveries.Framework.ParentingSystem;
using DerailedDeliveries.Framework.Audio;
using DerailedDeliveries.Framework.StateMachine.States;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables
{
    /// <summary>
    /// A <see cref="Interactable"/> class that is used for all grabbable <see cref="Interactable"/>s.
    /// </summary>
    public abstract class Grabbable : Interactable
    {
        [SerializeField]
        private float _maxGroundCheckDistance = 5f;

        [SerializeField]
        private Interactor _originInteractor;

        /// <summary>
        /// Returns the <see cref="Interactor"/> of this Interactable, will be null if not being
        /// targeted/interacted with.
        /// </summary>
        public Interactor OriginInteractor => _originInteractor;

        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private protected bool IsBeingInteracted { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="connection"><inheritdoc/></param>
        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);

            PlaceOnGround();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfInteractable(Interactor interactor)
            => base.CheckIfInteractable(interactor) && (!IsBeingInteracted || interactor == _originInteractor);

        [Server]
        private protected override bool Interact(Interactor interactor)
        {
            if (!base.Interact(interactor))
                return false;

            UseGrabbable(interactor);
            return true;
        }

        [Server]
        private protected virtual void UseGrabbable(Interactor interactor)
        {
            if (!IsBeingInteracted)
            {
                UpdateInteractionStatus(interactor, true);
                interactor.UpdateInteractingTarget(interactor.Owner, this, IsBeingInteracted);

                PlayGrabbableSound(false);

                NetworkObject.SetParent(interactor.GrabbingAnchor);
                transform.localPosition = Vector3.zero;

                return;
            }

            UpdateInteractionStatus(null, false);

            interactor.UpdateInteractingTarget(interactor.Owner, null, IsBeingInteracted);
            PlaceOnGround();
            return;
        }

        /// <summary>
        /// A function that updates the interaction information of this <see cref="Grabbable"/>.
        /// </summary>
        /// <param name="interactor">The current <see cref="Interactor"/> of this <see cref="Grabbable"/>.</param>
        /// <param name="isBeingInteracted">The new IsBeingInteracted bool state.</param>
        public void UpdateInteractionStatus(Interactor interactor, bool isBeingInteracted)
        {
            IsBeingInteracted = isBeingInteracted;
            _originInteractor = interactor;
        }

        /// <summary>
        /// A function that returns the new location if its <see cref="Transform"/> were to match the
        /// given target <see cref="Transform"/>.
        /// </summary>
        /// <param name="target">The target transform.</param>
        /// <returns>The new location that the Grabbable would be.</returns>
        public virtual Vector3 GetPositionOnGround(Vector3 position)
        {
            Physics.Raycast(position, Vector3.down, out RaycastHit hit, _maxGroundCheckDistance);

            return hit.point + new Vector3(0, BoxCollider.size.y * .5f, 0);
        }

        /// <summary>
        /// A function that places the <see cref="Grabbable"/> on the ground, can be used for after snapping to a
        /// transform.
        /// </summary>
        public virtual void PlaceOnGround()
        {
            if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _maxGroundCheckDistance))
                return;

            transform.position = hit.point + new Vector3(0, BoxCollider.size.y * .5f, 0);

            if (ObjectParentUtils.TryGetObjectParent(hit.collider.gameObject, out ObjectParent objectParent))
            {
                objectParent.SetParent(NetworkObject);
                PlayGrabbableSound(true);
                return;
            }

            NetworkObject.UnsetParent();
            PlayGrabbableSound(true);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void PlayGrabbableSound(bool isDrop)
        {
            if (StateMachine.StateMachine.Instance.CurrentState is not GameState)
                return;

            AudioCollectionTypes collectionType = isDrop 
                ? AudioCollectionTypes.DropGrabbable 
                : AudioCollectionTypes.GrabGrabbable;

            AudioSystem.Instance.PlayRandomSoundEffectOfType(collectionType, true, 1f);
        }
    }
}
