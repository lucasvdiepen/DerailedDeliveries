using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Level;
using DG.Tweening;
using DG.Tweening.Core;
using FishNet.Object;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    /// <summary>
    /// A class that is responsible for handling the delivery of <see cref="BoxGrabbable"/>'s.
    /// On succesfull delivery gets tracked by the <see cref="LevelTracker"/>.
    /// </summary>
    public class DeliveryBeltInteractable : Interactable
    {
        [SerializeField]
        private Transform _startTransform;

        [SerializeField]
        private Transform _endTransform;

        [SerializeField]
        private float _beltDuration = 2;

        [SerializeField]
        private TrainStation _parentStation;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfInteractable(Interactor interactor) => 
            base.CheckIfInteractable(interactor) && interactor.InteractingTarget != null;

        [Server]
        private protected override bool Interact(Interactor interactor) => false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="target"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        [Server]
        public override bool Interact(UseableGrabbable target)
        {
            if (target is not BoxGrabbable deliveryTarget)
                return false;

            Interactor originInteractor = deliveryTarget.OriginInteractor;
            originInteractor.UpdateInteractingTarget(originInteractor.Owner, null, false);

            deliveryTarget.NetworkObject.SetParent(this);
            deliveryTarget.transform.position = _startTransform.position;

            Vector3 startPos = deliveryTarget.GetPositionOnGround(_startTransform.position);
            Vector3 endPos = new Vector3(_endTransform.position.x, startPos.y, _endTransform.position.z);

            PackageData package = target.GetComponent<PackageData>();

            Tween packageTween = DOTween.To
                (
                    () => startPos, 
                    (Vector3 newPos) => { package.transform.position = newPos; }, 
                    endPos, 
                    _beltDuration
                ).OnComplete(() => CompleteDelivery(package));

            return true;
        }

        [Server]
        private void CompleteDelivery(PackageData package) =>
            LevelTracker.Instance.HandlePackageDelivery(package, _parentStation.StationID);
    }
}
