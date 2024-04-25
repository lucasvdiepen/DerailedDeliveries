using FishNet.Object.Synchronizing;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.DamageRepairManagement;
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

        /// <summary>
        /// A getter to get the ChaosSpeedMultiplierThreshold.
        /// </summary>
        public float ChaosSpeedMultiplierThreshold => _chaosSpeedMultiplierThreshold;

        /// <summary>
        /// A getter that returns the seconds that are left on the timer.
        /// </summary>
        public float TimeRemaining => _timer.Remaining;

        /// <summary>
        /// An action that broadcasts when the timer is updated and the new time that comes with it.
        /// </summary>
        public Action<float> OnTimerUpdated;

        /// <summary>
        /// An action that brodcasts when the timer is completed.
        /// </summary>
        public Action OnTimerCompleted;

        [SyncObject]
        private readonly SyncTimer _timer = new();

        [ContextMenu("Set Timer To Zero")]
        public void setTimerToZero() => _timer.Update(_timer.Remaining - 0.1f);

        [ContextMenu("Reset Timer")]
        public void ResetTimer() => _timer.StartTimer(_baseTime);

        [ContextMenu("Decrease timer")]
        public void DecreaseTimer() => _timer.StartTimer(_timer.Remaining - 10);

        [ContextMenu("Increase timer")]
        public void IncreaseTimer() => _timer.StartTimer(_timer.Remaining + 10);

        public override void OnStartServer()
        {
            base.OnStartServer();

            _timer.StartTimer(_baseTime);


        }

        /// <summary>
        /// A function that updates the time when the train arrives at a station.
        /// </summary>
        public void OnStationReached()
        {
            if (_timer.Paused)
                return;

            _timer.StartTimer(_timer.Remaining + _stationArrivalTimeBonus);

            OnTimerUpdated?.Invoke(_timer.Remaining);
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

            if(_timer.Remaining <= _chaosSpeedMultiplierThreshold)
                ApplyChaosMultiplier();
        }

        private void ApplyChaosMultiplier()
        {
            TrainDamageable[] damageables = FindObjectsOfType<TrainDamageable>();

            int damageablesCount = damageables.Length;
            for (int i = 0; i < damageablesCount; i++)
                damageables[i].ApplyChaosMultiplier(_chaosSpeedMultiplier);
        }
    }
}