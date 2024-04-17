using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Gameplay.Level;

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
        private float _beltSpeed = 1;

        [SerializeField]
        private TrainStation _parentStation;

        private Dictionary<PackageData, float> _packagesOnBelt = new();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfInteractable(Interactor interactor) => 
            base.CheckIfInteractable(interactor) && interactor.InteractingTarget != null;
        
        private protected override bool Interact(Interactor interactor) => false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="target"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool Interact(UseableGrabbable target)
        {
            if (target is not BoxGrabbable deliveryTarget)
                return false;

            Interactor originInteractor = deliveryTarget.OriginInteractor;
            originInteractor.UpdateInteractingTarget(originInteractor.Owner, null, false);

            deliveryTarget.NetworkObject.SetParent(this);
            deliveryTarget.transform.position = _startTransform.position;


            Vector3 startPos = deliveryTarget.GetPositionOnGround(_startTransform);
            Vector3 endPos = new Vector3(_endTransform.position.x, startPos.y, _endTransform.position.z);

            PackageData package = target.GetComponent<PackageData>();
            _packagesOnBelt.Add(package, 0);

            StartCoroutine(LerpBoxToPosition(package, startPos, endPos));
            
            return true;
        }

        private IEnumerator LerpBoxToPosition(PackageData package, Vector3 startPos, Vector3 endPos)
        {
            while(_packagesOnBelt.ContainsKey(package) && _packagesOnBelt[package] < 1)
            {
                float lerpFloat = _packagesOnBelt[package];
                lerpFloat = Mathf.Clamp01(lerpFloat + (Time.deltaTime * _beltSpeed));
                _packagesOnBelt[package] = lerpFloat;

                package.transform.position = Vector3.Lerp(startPos, endPos, lerpFloat);

                yield return null;
            }

            _packagesOnBelt.Remove(package);
            CompleteDelivery(package);
            yield return null;
        }

        private void CompleteDelivery(PackageData package) =>
            LevelTracker.Instance.HandlePackageDelivery(package, _parentStation.StationID);
    }
}
