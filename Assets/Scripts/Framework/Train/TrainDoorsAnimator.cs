using UnityEngine;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for opening and closing train doors.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class TrainDoorsAnimator : MonoBehaviour
    {
        private int _doorsOpenAnimationHash;
        private int _doorsCloseAnimationHash;

        private Animator _doorAnimator;

        private void Awake() => _doorAnimator = GetComponent<Animator>();

        private void OnEnable() => TrainStationController.Instance.OnParkStateChanged += HandleParkStateChanged;

        private void OnDisable() => TrainStationController.Instance.OnParkStateChanged -= HandleParkStateChanged;

        private void Start()
        {
            _doorsOpenAnimationHash = Animator.StringToHash("DoorsOpen");
            _doorsCloseAnimationHash = Animator.StringToHash("DoorsClose");
        }

        private void HandleParkStateChanged(bool newParkState) 
            => _doorAnimator.SetTrigger(newParkState ? _doorsOpenAnimationHash : _doorsCloseAnimationHash);
    }
}
