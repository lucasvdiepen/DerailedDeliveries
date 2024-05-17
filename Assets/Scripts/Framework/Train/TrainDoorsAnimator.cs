using UnityEngine;

using DerailedDeliveries.Framework.StateMachine;
using DerailedDeliveries.Framework.StateMachine.States;

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

        private void OnEnable() => StateMachine.StateMachine.Instance.OnStateChanged += HandleStateChanged;

        private void HandleStateChanged(State state)
        {
            if (state is not GameState)
                return;

            TrainStationController.Instance.OnParkStateChanged += HandleParkStateChanged;
            HandleParkStateChanged(true);
        }

        private void OnDisable()
        {
            if(TrainStationController.Instance != null) 
                TrainStationController.Instance.OnParkStateChanged -= HandleParkStateChanged;

            if(StateMachine.StateMachine.Instance != null)
                StateMachine.StateMachine.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void Start()
        {
            _doorsOpenAnimationHash = Animator.StringToHash("DoorsOpen");
            _doorsCloseAnimationHash = Animator.StringToHash("DoorsClose");
        }

        private void HandleParkStateChanged(bool newParkState)
            => _doorAnimator.SetTrigger(newParkState ? _doorsOpenAnimationHash : _doorsCloseAnimationHash);
    }
}
