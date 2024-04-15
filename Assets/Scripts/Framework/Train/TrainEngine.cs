using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Cinemachine;
using System;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.Camera;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for controlling the trains engine.
    /// </summary>
    [RequireComponent(typeof(TrainController))]
    public class TrainEngine : NetworkAbstractSingleton<TrainEngine>
    {
        [Header("Engine config")]
        [Tooltip("Friction value used as an opposing force when train is traveling.")]
        [SerializeField]
        private float _friction = .25f;

        [Tooltip("Separate friction value used when engine is turned off.")]
        [SerializeField]
        private float _standbyFriction = .1f;

        [SerializeField, Space]
        private float _brakeDuration = 1.5f;
        
        [Header("Acceleration levels")]
        [SerializeField]
        private float _high = 2;

        [SerializeField]
        private float _medium = 1;

        [SerializeField]
        private float _low = .5f;

        /// <summary>
        /// Current train engine state.
        /// </summary>
        public TrainEngineState EngineState 
            { get; private set; } = TrainEngineState.Active;

        /// <summary>
        /// Current train speed proportionally based on the length of the current spline.
        /// </summary>
        public float CurrentVelocity => CurrentSpeed / _trainController.SplineLength;

        /// <summary>
        /// A precalculated speed value which is the fastest the train can travel using the <see cref="_high"/> gear.
        /// </summary>
        public float MaxSpeed { get; private set; }

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
        public Action<TrainEngineState, TrainEngineState> OnEngineStateChanged;

        /// <summary>
        /// Invokes when train speed state is changed.
        /// </summary>
        public Action<int> OnSpeedStateChanged;

        #region SyncVars
        /// <summary>
        /// The raw train speed used to proportionally calculate <see cref="CurrentVelocity"/>. 
        /// </summary>
        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable), HideInInspector]
        public float CurrentSpeed { get; private set; }

        /// <summary>
        /// The current acceleration value that gets added to the <see cref="CurrentSpeed"/>.
        /// </summary>
        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable), HideInInspector]
        public float CurrentEngineAcceleration { get; private set; }
        
        /// <summary>
        /// Index used to handle switching between different levels of acceleration / deceleration.
        /// </summary>
        [field: SyncVar(Channel = FishNet.Transporting.Channel.Reliable, OnChange = nameof(OnSpeedStateChange)), HideInInspector]
        public int CurrentSpeedIndex { get; private set; }
        #endregion

        public const int SPEED_VALUES_COUNT = 3;

        private TrainController _trainController;
        private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

        private Dictionary<int, float> _speedValues;

        private float _brakeTimer;
        private float _startFriction;
        private float _startCameraNoiseAmplitude;
        
        private bool _isBraking;

        private void Awake()
            => _trainController = GetComponent<TrainController>();

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

            _startFriction = _friction;
            MaxSpeed = _speedValues[SPEED_VALUES_COUNT] / _friction;

            CinemachineVirtualCamera trainCamera = CameraManager.Instance.TrainCamera;
            _multiChannelPerlin = trainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            
            _startCameraNoiseAmplitude = _multiChannelPerlin.m_AmplitudeGain;
        }

        #region ServerRPCS
        /// <summary>
        /// Used to toggle if the engine should be on/off.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void ToggleEngineState()
            => OnTrainEngineStateChanged((TrainEngineState)(EngineState == TrainEngineState.Inactive ? 1 : 0));

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
            int newCurrentSpeed = CurrentSpeedIndex + (increment ? 1 : -1);
            CurrentSpeedIndex = Mathf.Clamp(newCurrentSpeed, -SPEED_VALUES_COUNT, SPEED_VALUES_COUNT);
           
            CurrentEngineAcceleration = _speedValues[CurrentSpeedIndex];
            OnTrainEngineStateChanged(TrainEngineState.Active);
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
        private void OnTrainEngineStateChanged(TrainEngineState newState)
        {
            EngineState = newState;
            OnDirectionChanged?.Invoke(CurrentSplitDirection);

            // If engine is set to inactive, reset train acceleration.
            if(newState == TrainEngineState.Inactive)
            {
                _friction = _standbyFriction;

                CurrentSpeedIndex = 0;
                CurrentEngineAcceleration = _speedValues[CurrentSpeedIndex];
            }
            else
            {
                _friction = _startFriction;
            }
        }
        #endregion

        private void Update()
        {
            if (!IsServer)
                return;
         
            UpdateCurrentSpeed();
        }

        /// <summary>
        /// Internally used to update current speed.
        /// </summary>
        private void UpdateCurrentSpeed()
        {
            if (_isBraking)
            {
                UpdateBraking();
                return;
            }

            CurrentSpeed -= CurrentSpeed * _friction * Time.deltaTime;
            CurrentSpeed += CurrentEngineAcceleration * Time.deltaTime;

            bool forwardCheck = CurrentSpeed > 0 && CurrentSpeedIndex < 0 && Mathf.Abs(CurrentSpeed) < 0.1f;
            bool backwardCheck = CurrentSpeed < 0 && CurrentSpeedIndex > 0 && Mathf.Abs(CurrentSpeed) < 0.1f;

            // Restart brake timer if train crosses from negative speed to positive or reversed.
            if (forwardCheck || backwardCheck)
            {
                _isBraking = true;
                _brakeTimer = _brakeDuration;
                CurrentSpeed = 0;
            }

            // Adjust Cinemachine noise amplitude gain based on current speed
            float amplitudeGain = Mathf.Lerp(0f, _startCameraNoiseAmplitude, Mathf.Abs(CurrentSpeed) / MaxSpeed);
            _multiChannelPerlin.m_AmplitudeGain = amplitudeGain;
        }

        private void UpdateBraking()
        {
            if (!_isBraking)
                return;

            _brakeTimer -= Time.deltaTime;

            if (_brakeTimer > 0)
                return;
            
            _isBraking = false;
            _brakeTimer = 0f;
        }

        private void OnSpeedStateChange(int previousValue, int newValue, bool asServer)
            => OnSpeedStateChanged?.Invoke(newValue);
    }
}