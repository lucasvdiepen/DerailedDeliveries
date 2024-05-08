using UnityEngine;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for opening and closing train doors.
    /// </summary>
    public class TrainDoorsAnimator : MonoBehaviour
    {
        private int _doorsOpenAnimationHash;
        private int _doorsCloseAnimationHash;

        private Animator[] _doorAnimators;

        private void Awake() => _doorAnimators = GetComponentsInChildren<Animator>();

        private void OnEnable() => TrainStationController.Instance.OnParkStateChanged += HandleParkStateChanged;

        private void OnDisable() => TrainStationController.Instance.OnParkStateChanged -= HandleParkStateChanged;

        private void Start()
        {
            _doorsOpenAnimationHash = Animator.StringToHash("DoorsOpen");
            _doorsCloseAnimationHash = Animator.StringToHash("DoorsClose");
        }

        private void HandleParkStateChanged(bool newParkState)
        {
            int doorsAmount = _doorAnimators.Length;

            for (int i = 0; i < doorsAmount; i++)
                _doorAnimators[i].SetTrigger(newParkState ? _doorsOpenAnimationHash : _doorsCloseAnimationHash);
        }
    }
}
