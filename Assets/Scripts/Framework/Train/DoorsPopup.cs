using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DerailedDeliveries.Framework.PopupManagement;
using DerailedDeliveries.Framework.StateMachine;
using DerailedDeliveries.Framework.StateMachine.States;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// A <see cref="Popup"/> class which handles the doors popup.
    /// </summary>
    public class DoorsPopup : Popup
    {
        [SerializeField]
        private Image _popupImage;

        [SerializeField]
        private Sprite _doorsOpenedSprite;

        [SerializeField]
        private Sprite _doorsClosedSprite;

        [SerializeField]
        private float _popupDuration = 5f;

        private Coroutine _closePopupCoroutine;

        private void OnEnable() => StateMachine.StateMachine.Instance.OnStateChanged += HandleStateChanged;

        private void OnDisable()
        {
            if(StateMachine.StateMachine.Instance != null)
                StateMachine.StateMachine.Instance.OnStateChanged -= HandleStateChanged;

            if (TrainStationController.Instance != null)
                TrainStationController.Instance.OnParkStateChanged -= OnParkStateChanged;
        }

        private void HandleStateChanged(State state)
        {
            if(state is not GameState)
                return;

            TrainStationController.Instance.OnParkStateChanged += OnParkStateChanged;
            OnParkStateChanged(TrainStationController.Instance.IsParked);
        }

        private void OnParkStateChanged(bool isParked)
        {
            if (_closePopupCoroutine != null)
                StopCoroutine(_closePopupCoroutine);

            _popupImage.sprite = isParked
                ? _doorsOpenedSprite
                : _doorsClosedSprite;

            Show();

            _closePopupCoroutine = StartCoroutine(ClosePopupDelay());
        }

        private IEnumerator ClosePopupDelay()
        {
            yield return new WaitForSeconds(_popupDuration);
            Close();
        }
    }
}