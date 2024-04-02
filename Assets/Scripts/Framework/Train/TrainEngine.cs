using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using FishNet.Object;
using DG.Tweening;
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
        private float _maxHighSpeed = 7.5f;

        [SerializeField]
        private float _maxMediumSpeed = 5f;

        [SerializeField]
        private float _maxLowSpeed = 2.5f;

        [SerializeField, Space]
        private float _accelerationDuration = 10f;

        [SerializeField]
        private Ease _accelerationEase = Ease.Linear;

        /// <summary>
        /// Current train engine state.
        /// </summary>
        public TrainEngineState EngineState 
            { get; private set; } = TrainEngineState.OnStandby;

        /// <summary>
        /// Current train speed type.
        /// </summary>
        public TrainEngineSpeedTypes CurrentEngineSpeedType 
            { get; private set; } = TrainEngineSpeedTypes.Still;

        /// <summary>
        /// Current train speed type.
        /// </summary>
        public TrainEngineSpeedTypes CurrentTargetEngineSpeedType 
            { get; private set; } = TrainEngineSpeedTypes.Still;

        /// <summary>
        /// Current train velocity speed.
        /// </summary>
        public float CurrentVelocity => _currentSpeed / _trainController.SplineLength;

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
        /// Invoked when the train speed is changed.
        /// </summary>
        public Action<TrainEngineSpeedTypes> OnSpeedChanged;

        /// <summary>
        /// Invoked when the train target speed is changed.
        /// </summary>
        public Action<TrainEngineSpeedTypes> OnTargetSpeedChanged;

        /// <summary>
        /// Invoked when the train engine state is changed.
        /// </summary>
        public Action<TrainEngineState> OnEngineStateChanged;

        private Dictionary<TrainEngineSpeedTypes, float> _speedValues;

        [HideInInspector]
        [SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private float _currentSpeed;
        
        private float _speedTypesCount;

        [SerializeField] private bool isLerping;

        private Tween _speedTween;
        private TrainController _trainController;

        private void Awake() => _trainController = GetComponent<TrainController>();

        private void Start()
        {
            _speedTypesCount = Enum.GetValues(typeof(TrainEngineSpeedTypes)).Length - 1;

            // Initialize speed values dictionary.
            _speedValues = new Dictionary<TrainEngineSpeedTypes, float>()
            {
                {TrainEngineSpeedTypes.High, _maxHighSpeed },
                {TrainEngineSpeedTypes.Low, _maxLowSpeed },
                {TrainEngineSpeedTypes.Medium, _maxMediumSpeed },
                
                {TrainEngineSpeedTypes.Still, 0 },

                {TrainEngineSpeedTypes.LowReverse, -_maxLowSpeed },
                {TrainEngineSpeedTypes.MediumReverse, -_maxMediumSpeed },
                {TrainEngineSpeedTypes.HighReverse, -_maxHighSpeed },
            };
        }

        /// <summary>
        /// Helper method for checking if the train is moving forwards.
        /// </summary>
        /// <returns>True if the train is moving forward.</returns>
        public bool IsTraveling()
            => (int)CurrentEngineSpeedType > 3 || (int)CurrentTargetEngineSpeedType > 3;

        /// <summary>
        /// Helper method for checking if the train is moving backwards.
        /// </summary>
        /// <returns>True if the train moving backward.</returns>
        public bool IsTravelingReverse()
            => (int)CurrentEngineSpeedType < 3 || (int)CurrentTargetEngineSpeedType < 3;
        
        #region ServerRPCS
        /// <summary>
        /// Used to toggle direction of upcomming rail split.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void ToggleTrainDirection()
            => OnTrainDirectionChanged(!CurrentSplitDirection);

        /// <summary>
        /// Used to set the train engine state.
        /// </summary>
        /// <param name="newState">New state of the train engine.</param>
        [ServerRpc(RequireOwnership = false)]
        public void SetTrainEngineState(TrainEngineState newState)
            => OnTrainEngineStateChanged(newState);

        /// <summary>
        /// Method for increasing/decreasing train speed level.
        /// </summary>
        /// <param name="increment">Whether to increase or decrease speed.</param>
        [ServerRpc(RequireOwnership = false)]
        public void AdjustSpeed(bool increment)
        {
            TrainEngineSpeedTypes newTargetSpeed = (TrainEngineSpeedTypes)Mathf.Clamp(
                (int)CurrentTargetEngineSpeedType + (increment ? 1 : -1),
                0,
                _speedTypesCount
            );

            OnTrainTargetSpeedChanged(newTargetSpeed);
            TweenTrainSpeed(CurrentTargetEngineSpeedType);
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
            OnEngineStateChanged?.Invoke(newState);
        }

        [ObserversRpc(BufferLast = true, RunLocally = true)]
        private void OnTrainTargetSpeedChanged(TrainEngineSpeedTypes newSpeed)
        {
            CurrentTargetEngineSpeedType = newSpeed;
            OnTargetSpeedChanged?.Invoke(CurrentTargetEngineSpeedType);
        }

        [ObserversRpc(BufferLast = true, RunLocally = true)]
        private void OnTrainSpeedChanged(TrainEngineSpeedTypes newSpeed)
        {
            CurrentEngineSpeedType = newSpeed;
            OnSpeedChanged?.Invoke(CurrentEngineSpeedType);
        }
        #endregion

        /// <summary>
        /// Internally used to tween between different levels of speed.
        /// </summary>
        /// <param name="targetEngineSpeedType">New target speed to tween towards.</param>
        [Server]
        private void TweenTrainSpeed(TrainEngineSpeedTypes targetEngineSpeedType, bool checkIsBraking = true)
        {
            _speedTween.Kill();

            float currentMaxSpeed = _speedValues[targetEngineSpeedType];
            float duration = _accelerationDuration;
            
            if (CurrentEngineSpeedType != TrainEngineSpeedTypes.Still && checkIsBraking)
                currentMaxSpeed = 0;

            _speedTween = DOTween.To(() => _currentSpeed, x => _currentSpeed = x, currentMaxSpeed, duration)
                .SetEase(_accelerationEase)
                .OnStart(() => isLerping = true);

            _speedTween.OnComplete(() =>
            {
                float reverseSpeed = _speedValues[targetEngineSpeedType];
                if (currentMaxSpeed != reverseSpeed) 
                {
                    TweenTrainSpeed(targetEngineSpeedType, false);
                    print("Switch to " + targetEngineSpeedType);
                    return;
                }

                TrainEngineState newEngineState = targetEngineSpeedType == TrainEngineSpeedTypes.Still
                    ? TrainEngineState.OnStandby : TrainEngineState.On;

                isLerping = false;
                SetTrainEngineState(newEngineState);
                OnTrainSpeedChanged(targetEngineSpeedType);
            });

            _speedTween.Play();
        }

    }
}
