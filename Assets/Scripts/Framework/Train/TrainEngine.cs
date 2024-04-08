using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.Utils;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for controlling the trains engine.
    /// </summary>
    [RequireComponent(typeof(TrainController))]
    public class TrainEngine : NetworkAbstractSingleton<TrainEngine>
    {
        [SerializeField]
        private float _maxSpeed = 15f;

        [SerializeField]
        private float _friction = .25f;

        [Header("Acceleration / deceleration levels")]
        [SerializeField]
        private float _high = 2;

        [SerializeField]
        private float _medium = 1;

        [SerializeField]
        private float _low = .5f;

        [SerializeField]
        private float _brakeDuration = 3;

        [SerializeField]
        private int _currentEngineSpeedIndex;

        [SerializeField]
        private float _currentEngineSpeed;

        [SerializeField]
        private float _currentEngineMaxSpeed;

        /// <summary>
        /// Current train engine state.
        /// </summary>
        public TrainEngineStates EngineState 
            { get; private set; } = TrainEngineStates.OnStandby;

        /// <summary>
        /// Current train velocity speed.
        /// </summary>
        public float CurrentVelocity => CurrentSpeed / _trainController.SplineLength;

        /// <summary>
        /// Determines the chosen track for the next possible rail split.<br></br>
        /// <br>False = left.</br>
        /// <br>True = right.</br>
        /// </summary>
        public bool CurrentSplitDirection { get; set; }

        /// <summary>
        /// Invokes when train direction is changed.
        /// </summary>
        public Action<bool> OnDirectionChanged;

        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        public float CurrentSpeed { get; private set; }
        
        private TrainController _trainController;

        private Dictionary<int, float> _speedValues;

        private const int SPEED_VALUES_COUNT = 3;

        private void Awake() => _trainController = GetComponent<TrainController>();

        private void Start()
        {
            _speedValues = new Dictionary<int, float>()
            {
                {  0,    0      },
                {  1,   _low    },
                {  2,   _medium },
                {  3,   _high   },
                { -1, -_low     },
                { -2, -_medium  },
                { -3, -_high    },
            };
        }

        #region ServerRPCS
        /// <summary>
        /// Used to toggle direction of upcomming rail split.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void ToggleTrainDirection()
            => OnTrainDirectionChanged(!CurrentSplitDirection);

        /// <summary>
        /// Method for increasing/decreasing train speed level.
        /// </summary>
        /// <param name="increment">Whether to increase or decrease speed.</param>
        [ServerRpc(RequireOwnership = false)]
        public void AdjustSpeed(bool increment)
        {
            _currentEngineSpeedIndex += increment ? 1 : -1;
            _currentEngineSpeedIndex = Mathf.Clamp(_currentEngineSpeedIndex, -SPEED_VALUES_COUNT, SPEED_VALUES_COUNT);
           
            _currentEngineSpeed = _speedValues[_currentEngineSpeedIndex];
        }   
        #endregion;
        
        #region ObserverRPCS
        [ObserversRpc(BufferLast = true, RunLocally = true)]
        private void OnTrainDirectionChanged(bool newDirection)
        {
            CurrentSplitDirection = newDirection;
            OnDirectionChanged?.Invoke(CurrentSplitDirection);
        }
        #endregion

        private bool isWaiting = false;
        private float waitTimer = 0f;

        private void Update()
        {
            if (!IsServer)
                return;

            UpdateWaiting();

            if (isWaiting)
                return;

            CurrentSpeed -= CurrentSpeed * _friction * Time.deltaTime;

            CurrentSpeed += _currentEngineSpeed * Time.deltaTime;
            CurrentSpeed = Mathf.Clamp(CurrentSpeed, -_maxSpeed, _maxSpeed);

            bool forwardCheck = CurrentSpeed > 0 && _currentEngineSpeedIndex < 0 && Mathf.Abs(CurrentSpeed) < 0.1f;
            bool backwardCheck = CurrentSpeed < 0 && _currentEngineSpeedIndex > 0 && Mathf.Abs(CurrentSpeed) < 0.1f;

            if (forwardCheck || backwardCheck)
                StartWaitTimer();
        }

        private void UpdateWaiting()
        {
            if (isWaiting)
            {
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    isWaiting = false;
                    waitTimer = 0f;
                }
            }
        }

        private void StartWaitTimer()
        {
            isWaiting = true;
            waitTimer = _brakeDuration;
            CurrentSpeed = 0;
        }
    }
}