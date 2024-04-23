using FishNet.Object.Synchronizing;
using FishNet.Object;
using UnityEngine;
using TMPro;

using DerailedDeliveries.Framework.StateMachine.States;
using DerailedDeliveries.Framework.UI.TextUpdaters;

namespace DerailedDeliveries.Framework.Gameplay.Timer
{
    /// <summary>
    /// A class that is responsible for updating the Timer text in the <see cref="GameState"/>.
    /// </summary>
    public class TimerUpdater : NetworkBehaviour
    {
        [SerializeField]
        private TextUpdater _milliSecondsText;

        [SerializeField]
        private TextUpdater _secondsText;

        [SerializeField]
        private TextUpdater _minutesText;

        [SyncObject]
        private readonly SyncTimer _timer = new();

        private void Awake()
        {
            _timer.StartTimer(100f, false);
        }

        private void Update()
        {
            if (!_milliSecondsText || !_secondsText || !_minutesText)
                return;

            if (!_timer.Paused)
                UpdateTimer();
        }

        private void UpdateTimer()
        {
            _timer.Update(Time.deltaTime);

            UpdateText(_timer.Remaining);
        }

        private void UpdateText(float newTime)
        {
            int minutes = (int)(newTime / 60);
            int seconds = (int)(newTime % 60);
            int milliseconds = (int)(100 * (newTime % 1));

            _minutesText.ReplaceTag(GetIntString(minutes));
            _secondsText.ReplaceTag(GetIntString(seconds));
            _milliSecondsText.ReplaceTag(GetIntString(milliseconds));
        }

        private string GetIntString(int number)
        {
            return number < 10 
                ? "0" + number 
                : number.ToString();
        }
    }
}