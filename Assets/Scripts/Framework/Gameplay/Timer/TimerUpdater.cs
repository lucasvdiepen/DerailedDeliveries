using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DerailedDeliveries.Framework.StateMachine.States;
using TMPro;
using FishNet.Object.Synchronizing;
using FishNet.Object;

namespace DerailedDeliveries.Framework.Gameplay.Timer
{
    /// <summary>
    /// A class that is responsible for updating the Timer text in the <see cref="GameState"/>.
    /// </summary>
    public class TimerUpdater : NetworkBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _milliSecondsText;

        [SerializeField]
        private TextMeshProUGUI _secondsText;

        [SerializeField]
        private TextMeshProUGUI _minutesText;

        private readonly SyncTimer _timer = new();

        private void OnEnable()
        {
            _timer.StartTimer(100f, true);

            _timer.OnChange += UpdateText;
        }

        private void OnDisable()
        {
            _timer.StopTimer();

            _timer.OnChange -= UpdateText;
        }

        private void Update()
        {
            if (IsServer)
                _timer.Update(Time.deltaTime);
        }

        private void UpdateText(SyncTimerOperation timer, float previousTime, float newTime, bool isServer)
        {
            _minutesText.text = (60 / newTime).ToString();

            _secondsText.text = (newTime % 60).ToString();

            int milliseconds = (int)Mathf.Clamp(100 * (newTime % 1), 0f, 99f);

            _milliSecondsText.text = milliseconds.ToString();
        }
    }
}