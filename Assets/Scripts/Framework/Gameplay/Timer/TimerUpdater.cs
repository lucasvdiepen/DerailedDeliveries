using FishNet.Object.Synchronizing;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.StateMachine.States;
using DerailedDeliveries.Framework.Utils;

namespace DerailedDeliveries.Framework.Gameplay.Timer
{
    /// <summary>
    /// A class that is responsible for updating the timer text in the <see cref="GameState"/>.
    /// </summary>
    public class TimerUpdater : NetworkAbstractSingleton<TimerUpdater>
    {
        [SerializeField]
        private float _baseTime = 120f;

        [SerializeField]
        private float _stationArrivalTimeBonus = 40f;

        [SerializeField]
        private float _chaosSpeedMultiplierThreshold = 60f;

        [SerializeField]
        private float _chaosSpeedMultiplier = 1.5f;

        public Action<float> OnTimerUpdated;

        public Action OnTimerCompleted;

        [SyncObject]
        private readonly SyncTimer _timer = new();

        public override void OnStartServer()
        {
            base.OnStartServer();

            _timer.StartTimer(_baseTime);
        }

        /// <summary>
        /// A function that starts/stops the timer.
        /// </summary>
        /// <param name="newTime">The new time for the timer, is automatically 0 when not entered.</param>
        public void ToggleTimer(float newTime = 0f)
        {
            if(!_timer.Paused || newTime == 0)
                _timer.StopTimer();
            else
                _timer.StartTimer(newTime, false);
        }

        private void Update()
        {
            if (_timer.Paused)
                return;

            _timer.Update(Time.deltaTime);
            OnTimerUpdated?.Invoke(_timer.Remaining);

            if (_timer.Remaining <= 0)
                OnTimerCompleted?.Invoke();
        }
    }
}