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
    public class DeliveryBelt : Interactable
    {
        [SerializeField]
        private Transform _startTransform;

        [SerializeField]
        private Transform _endTransform;

        [SerializeField]
        private float _beltSpeed = 1;

        [SerializeField]
        private TrainStation _parentStation;

        private Dictionary<BoxGrabbable, float> _interactablesOnBelt = new();

        public override bool CheckIfInteractable(Interactor interactor) => 
            base.CheckIfInteractable(interactor) && interactor.InteractingTarget != null;
        
        private protected override bool Interact(Interactor interactor) => false;

        public override bool Interact(UseableGrabbable target)
        {
            if (!(target is BoxGrabbable deliveryTarget))
                return false;

            Interactor originInteractor = deliveryTarget.OriginInteractor;
            originInteractor.UpdateInteractingTarget(originInteractor.Owner, null, false);

            deliveryTarget.NetworkObject.SetParent(this);
            deliveryTarget.transform.position = _startTransform.position;

            _interactablesOnBelt.Add(deliveryTarget, 0);

            Vector3 startPos = deliveryTarget.GetPositionOnGround(_startTransform);
            Vector3 endPos = new Vector3(_endTransform.position.x, startPos.y, _endTransform.position.z);
            StartCoroutine(LerpBoxToPosition(deliveryTarget, startPos, endPos));
            
            return true;
        }

        private IEnumerator LerpBoxToPosition(BoxGrabbable target, Vector3 startPos, Vector3 endPos)
        {
            while(_interactablesOnBelt.ContainsKey(target) && _interactablesOnBelt[target] < 1)
            {
                float lerpFloat = _interactablesOnBelt[target];
                lerpFloat = Mathf.Clamp01(lerpFloat + (Time.deltaTime * _beltSpeed));
                _interactablesOnBelt[target] = lerpFloat;

                target.transform.position = Vector3.Lerp(startPos, endPos, lerpFloat);

                yield return null;
                continue;
            }

            _interactablesOnBelt.Remove(target);
            CompleteDelivery(target);
            yield return null;
        }

        private void CompleteDelivery(BoxGrabbable delivery) =>
            LevelTracker.Instance.HandlePackageDelivery(delivery, _parentStation.StationID);
    }
}
