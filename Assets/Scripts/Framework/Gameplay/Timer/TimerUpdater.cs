using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.DamageRepairManagement;
using DerailedDeliveries.Framework.StateMachine.States;
using DerailedDeliveries.Framework.GameManagement;
using DerailedDeliveries.Framework.Station;
using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.Train;
using DerailedDeliveries.Framework.StateMachine;

namespace DerailedDeliveries.Framework.Gameplay.Timer
{
    /// <summary>
    /// A class that is responsible for updating the timer in the <see cref="GameState"/>.
    /// </summary>
    public class TimerUpdater : NetworkAbstractSingleton<TimerUpdater>
    {
        public float newTime;

        [ContextMenu("Set Timer Time")]
        private void SetTimerTime() => _timer.StartTimer(newTime);

        [SerializeField]
        private float _baseTime = 240f;

        [SerializeField]
        private float _stationArrivalTimeBonus = 40f;

        [SerializeField]
        private float _chaosSpeedMultiplierThreshold = 60f;

        [SerializeField]
        private float _chaosSpeedMultiplier = 1.5f;

        [SerializeField]
        private List<int> _arrivedStations = new();

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
        /// An action that broadcasts when the timer is completed.
        /// </summary>
        public Action OnTimerCompleted;

        [SyncObject]
        private readonly SyncTimer _timer = new();

        private bool _isChaos;
        private bool _hasTimerStarted;
        private bool _isTimerFinished;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            _timer.OnChange += TimerUpdated;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStopClient()
        {
            base.OnStopClient();

            _timer.OnChange -= TimerUpdated;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();

            StateMachine.StateMachine.Instance.OnStateChanged += HandleStateChanged;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStopServer()
        {
            base.OnStopServer();

            _timer.StopTimer();

            if(StateMachine.StateMachine.Instance != null)
                StateMachine.StateMachine.Instance.OnStateChanged -= HandleStateChanged;

            if (TrainStationController.Instance != null)
                TrainStationController.Instance.OnParkStateChanged -= OnStationArrival;
        }

        private void HandleStateChanged(State state)
        {
            if(state is not GameState)
                return;

            _timer.StartTimer(_baseTime);

            TrainStationController.Instance.OnParkStateChanged += OnStationArrival;
            OnTimerCompleted += GameManager.Instance.EndGame;
        }

        /// <summary>
        /// A function that updates the time when the train arrives at a station.
        /// </summary>
        private void OnStationArrival(bool isParked)
        {
            if (_timer.Paused || !isParked)
                return;

            int stationIndex = StationManager.Instance.GetNearestStationIndex
                (
                    TrainStationController.Instance.CurrentTrainLocation
                );

            if (_arrivedStations.Contains(stationIndex))
                return;

            _arrivedStations.Add(stationIndex);

            _timer.StartTimer(_timer.Remaining + _stationArrivalTimeBonus);

            OnTimerUpdated?.Invoke(_timer.Remaining);
        }

        private void Update()
        {
            if (_timer.Paused || _isTimerFinished || !_hasTimerStarted)
                return;

            UpdateTimer();
        }

        private void UpdateTimer()
        {
            _timer.Update(Time.deltaTime);
            OnTimerUpdated?.Invoke(_timer.Remaining);

            if (_timer.Remaining <= 0)
            {
                _isTimerFinished = true;
                OnTimerCompleted?.Invoke();
            }

            if (!IsServer)
                return;

            if (_timer.Remaining <= _chaosSpeedMultiplierThreshold != _isChaos)
                ToggleChaosMultiplier();
        }

        private void TimerUpdated(SyncTimerOperation op, float prev, float next, bool asServer)
        {
            if (op != SyncTimerOperation.Start)
                return;

            _hasTimerStarted = true;
        }

        private void ToggleChaosMultiplier()
        {
            _isChaos = !_isChaos;

            TrainDamageable[] damageables = FindObjectsOfType<TrainDamageable>();

            int damageablesCount = damageables.Length;
            for (int i = 0; i < damageablesCount; i++)
                damageables[i].ToggleChaosMultiplier(_chaosSpeedMultiplier, _isChaos);
        }
    }
}