using UnityEngine;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for opening and closing doors.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class TrainDoorsAnimator : MonoBehaviour
    {
        private int _doorsOpenHash;
        private int _doorsCloseHash;

        private Animator _doorsAnimator;

        private void Awake() => _doorsAnimator = GetComponent<Animator>();

        private void OnEnable() => TrainStationController.Instance.OnParkStateChanged += HandleParkStateChanged;
        private void OnDisable() => TrainStationController.Instance.OnParkStateChanged -= HandleParkStateChanged;

        private void Start()
        {
            _doorsOpenHash = Animator.StringToHash("DoorsOpen");
            _doorsCloseHash = Animator.StringToHash("DoorsClose");
        }

        private void HandleParkStateChanged(bool newParkState)
            => _doorsAnimator.SetTrigger(newParkState ? _doorsOpenHash : _doorsCloseHash);
    }
}
