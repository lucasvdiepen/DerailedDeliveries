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
        private float _brakeDuration = 1.5f;
        
        [SerializeField]
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

        private Tween _speedTween;
        private Sequence _speedSequence;

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
        /// A return method for checking if the train is moving forwards.
        /// </summary>
        /// <returns>True if the train is moving forward.</returns>
        public bool IsTraveling()
            => (int)CurrentEngineSpeedType > 3 || (int)CurrentTargetEngineSpeedType > 3;

        /// <summary>
        /// A return method for checking if the train is moving backwards.
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
            BetterTweenTrainSpeed(newTargetSpeed);
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


        [Server]
        private void BetterTweenTrainSpeed(TrainEngineSpeedTypes targetSpeedType)
        {
            TrainEngineSpeedTypes lastSpeed = CurrentTargetEngineSpeedType;
            OnTrainTargetSpeedChanged(targetSpeedType);
            
            // Spam check.
            if(lastSpeed == CurrentTargetEngineSpeedType)
                return;

            _speedSequence.Kill();

            float duration = _accelerationDuration;
            float currentMaxSpeed = _speedValues[targetSpeedType];

            _speedSequence = DOTween.Sequence()
                .Append(DOTween.To(() => _currentSpeed, x => _currentSpeed = x, currentMaxSpeed, duration)
                    .SetEase(_accelerationEase));

            // Check if train should brake and tween to 0 first before tweening to desired speed type.
            if (ShouldBrake(targetSpeedType))
            {
                print("Prepend braking");
                _speedSequence.PrependInterval(1.5f);
                _speedSequence.Prepend(DOTween.To(() => _currentSpeed, x => _currentSpeed = x, 0, duration)
                    .SetEase(_accelerationEase));
                
            }
            
            _speedSequence.OnComplete(() =>
            {
                // Set correct engine state based on current target engine speed type.
                TrainEngineState newEngineState = targetSpeedType == TrainEngineSpeedTypes.Still
                    ? TrainEngineState.OnStandby : TrainEngineState.On;

                //Update all clients with new engine state and target engine speed type.
                SetTrainEngineState(newEngineState);
                OnTrainSpeedChanged(targetSpeedType);
            });

            _speedSequence.Play();
        }

        /// <summary>
        /// Internally used to tween between different levels of speed.
        /// </summary>
        /// <param name="targetSpeedType">New target speed to tween towards.</param>
        [Server]
        private void TweenTrainSpeed(TrainEngineSpeedTypes targetSpeedType, bool checkBrakes, bool skipSpamCheck = false)
        {
            TrainEngineSpeedTypes lastSpeed = CurrentTargetEngineSpeedType;
            OnTrainTargetSpeedChanged(targetSpeedType);

            // Spam check, return early when the input target speed is the same as the current target speed.
            if(!skipSpamCheck && lastSpeed == CurrentTargetEngineSpeedType)
                return;

            _speedTween.Kill();

            // Get current max speed and set tween duration.
            float currentMaxSpeed = _speedValues[targetSpeedType];
            float duration = _accelerationDuration;

            // Check if train should brake and tween to 0 first before tweening to desired speed type.
            /*if (ShouldBrake(targetSpeedType, checkBrakes))
                currentMaxSpeed = 0;*/

            // Tween train _currentSpeed to current max speed over 'duration' seconds.
            _speedTween = DOTween.To(() => _currentSpeed, x => _currentSpeed = x, currentMaxSpeed, duration)
                .SetEase(_accelerationEase);

            _speedTween.OnComplete(() =>
            {
                // Check if train needs to tween to desired speed value from speed type TrainEngineSpeedTypes.Still.
                float newCurrentSpeed = _speedValues[targetSpeedType];
                if (currentMaxSpeed != newCurrentSpeed) 
                {
                    // Tween to desired speed type from 0. Need to skip spam check to
                    // avoid early return since it has the same target speed type.
                    TweenTrainSpeed(targetSpeedType, false, true);
                    return;
                }

                // Set correct engine state based on current target engine speed type.
                TrainEngineState newEngineState = targetSpeedType == TrainEngineSpeedTypes.Still
                    ? TrainEngineState.OnStandby : TrainEngineState.On;

                //Update all clients with new engine state and target engine speed type.
                SetTrainEngineState(newEngineState);
                OnTrainSpeedChanged(targetSpeedType);
            });

            _speedTween.Play();
        }

        /// <summary>
        /// Internally used to check if train should brake to 0 speed.
        /// </summary>
        /// <param name="targetEngineSpeedType">The speed the train wants to move towards.</param>
        /// <param name="canBrake">Override bool to determine of train is allowed to brake.</param>
        /// <returns>True if train should brake to 0 speed.</returns>
        private bool ShouldBrake(TrainEngineSpeedTypes targetEngineSpeedType)
        {
            bool tryReverse = (int)targetEngineSpeedType < (int)TrainEngineSpeedTypes.Still;
            bool reverseCheck = tryReverse && (int)CurrentEngineSpeedType > 3;

            bool tryTravel = (int)targetEngineSpeedType > (int)TrainEngineSpeedTypes.Still;
            bool travelCheck = tryTravel && (int)CurrentEngineSpeedType < 3;

            return reverseCheck || travelCheck;
        }
    }
}