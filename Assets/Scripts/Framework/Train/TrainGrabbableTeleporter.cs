using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Utils.ObjectParenting;
using DerailedDeliveries.Framework.ParentingSystem;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// A class responsible for teleporting a <see cref="Grabbable"/> back to its origin if it is not in the train.
    /// </summary>
    [RequireComponent(typeof(Grabbable))]
    public class TrainGrabbableTeleporter : NetworkBehaviour
    {
        [SerializeField]
        private LayerMask _trainLayer;

        [SerializeField]
        private Transform _originTransform;

        private Grabbable _grabbable;

        private void Awake() => _grabbable = GetComponent<Grabbable>();

        private void OnTransformParentChanged()
        {
            if(!IsServer)
                return;

            if(ObjectParentUtils.TryGetObjectParent(gameObject, out ObjectParent objectParent))
            {
                if((_trainLayer.value & 1 << objectParent.gameObject.layer) != 0)
                    return;

                TeleportBack();
                return;
            }

            TeleportBack();
        }

        [Server]
        private void TeleportBack()
        {
            if(_grabbable.OriginInteractor != null)
                _grabbable.OriginInteractor.UpdateInteractingTarget(_grabbable.OriginInteractor.Owner, null, false);

            _grabbable.UpdateInteractionStatus(null, false);

            NetworkObject.UnsetParent();

            transform.position = _originTransform.position;
            _grabbable.PlaceOnGround();
        }
    }
}