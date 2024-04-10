using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Interactions.Grabbables;
using DerailedDeliveries.Framework.Gameplay.Player;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    public class DeliveryBelt : Interactable
    {
        [SerializeField]
        private Transform _startTransform;

        [SerializeField]
        private Transform _endTransform;

        [SerializeField]
        private float _beltSpeed = 1;

        private Dictionary<Interactable, float> _interactablesOnBelt = new();

        public override bool CheckIfInteractable(Interactor interactor) => 
            base.CheckIfInteractable(interactor) && interactor.InteractingTarget != null;
        
        private protected override bool Interact(Interactor interactor) => false;

        public override bool Interact(UseableGrabbable useableGrabbable)
        {
            if (!(useableGrabbable is BoxGrabbable boxGrabbable))
                return false;

            Interactor originInteractor = useableGrabbable.OriginInteractor;
            originInteractor.UpdateInteractingTarget(originInteractor.Owner, null, false);

            useableGrabbable.NetworkObject.SetParent(this);
            useableGrabbable.transform.position = _startTransform.position;

            _interactablesOnBelt.Add(useableGrabbable, 0);

            Vector3 startPos = useableGrabbable.GetPositionOnGround(_startTransform);
            Vector3 endPos = useableGrabbable.GetPositionOnGround(_endTransform);
            StartCoroutine(LerpBoxToPosition(useableGrabbable, startPos, endPos));

            return true;
        }

        private IEnumerator LerpBoxToPosition(UseableGrabbable target, Vector3 startPos, Vector3 endPos)
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
            Destroy(target.gameObject);
            yield return null;
        }
    }
}
