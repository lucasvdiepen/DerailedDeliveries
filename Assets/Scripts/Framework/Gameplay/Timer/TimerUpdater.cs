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

        [SyncObject]
        private readonly SyncTimer _timer = new();

        private void Awake()
        {
            _timer.StartTimer(100f, false);
        }

        private void Update()
        {
            if (IsServer)
                _timer.Update(Time.deltaTime);

            UpdateText(_timer.Remaining);
        }

        private void UpdateText(float newTime)
        {
            int minutes = (int)(newTime / 60);
            int seconds = (int)(newTime % 60);
            int milliseconds = (int)(100 * (newTime % 1));

            _minutesText.text = minutes.ToString();
            _secondsText.text = seconds.ToString();
            _milliSecondsText.text = milliseconds.ToString();
        }
    }
}