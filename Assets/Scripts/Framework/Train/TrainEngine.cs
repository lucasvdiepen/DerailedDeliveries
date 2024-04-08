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

        /// <summary>
        /// Current train engine state.
        /// </summary>
        public TrainEngineStates EngineState 
            { get; private set; } = TrainEngineStates.Active;

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

        /// <summary>
        /// Invokes when train engine state is changed.
        /// </summary>
        public Action<TrainEngineStates, TrainEngineStates> OnEngineStateChanged;

        #region SyncVars
        [field: HideInInspector]
        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        public float CurrentSpeed { get; private set; }

        [field: HideInInspector]
        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        public int CurrentSpeedIndex { get; private set; }

        [field: HideInInspector]
        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        public float CurrentEngineAcceleration { get; private set; }
        #endregion

        private TrainController _trainController;
        private Dictionary<int, float> _speedValues;

        private const int SPEED_VALUES_COUNT = 3;
        private bool isWaiting = false;
        private float waitTimer = 0f;

        private void Awake() => _trainController = GetComponent<TrainController>();

        private void Start()
        {
            _speedValues = new()
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
        /// Used to toggle if the engine should be on/off.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void ToggleEngineState()
            => OnTrainEngineStateChanged((TrainEngineStates)(EngineState == TrainEngineStates.Inactive ? 1 : 0));

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
            CurrentSpeedIndex += increment ? 1 : -1;
            CurrentSpeedIndex = Mathf.Clamp(CurrentSpeedIndex, -SPEED_VALUES_COUNT, SPEED_VALUES_COUNT);
           
            CurrentEngineAcceleration = _speedValues[CurrentSpeedIndex];
            OnTrainEngineStateChanged(TrainEngineStates.Active);
        }   
        #endregion;
        
        #region ObserverRPCS
        [ObserversRpc(BufferLast = true, RunLocally = true)]
        private void OnTrainDirectionChanged(bool newDirection)
        {
            CurrentSplitDirection = newDirection;
            OnDirectionChanged?.Invoke(CurrentSplitDirection);
        }

        [ObserversRpc(BufferLast = true, RunLocally = true)]
        private void OnTrainEngineStateChanged(TrainEngineStates newState)
        {
            EngineState = newState;
            OnDirectionChanged?.Invoke(CurrentSplitDirection);

            // If engine is set to inactive, reset train acceleration.
            if(newState == TrainEngineStates.Inactive)
            {
                CurrentSpeedIndex = 0;
                CurrentEngineAcceleration = _speedValues[CurrentSpeedIndex];
            }
        }
        #endregion

        private void Update()
        {
            if (!IsServer)
                return;

            UpdateWaiting();

            if (isWaiting)
                return;

            CurrentSpeed -= CurrentSpeed * _friction * Time.deltaTime;
            CurrentSpeed += CurrentEngineAcceleration * Time.deltaTime;

            bool forwardCheck = CurrentSpeed > 0 && CurrentSpeedIndex < 0 && Mathf.Abs(CurrentSpeed) < 0.1f;
            bool backwardCheck = CurrentSpeed < 0 && CurrentSpeedIndex > 0 && Mathf.Abs(CurrentSpeed) < 0.1f;

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