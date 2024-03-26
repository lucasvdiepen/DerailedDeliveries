using System.Collections.Generic;
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
    public class TrainEngine : AbstractSingleton<TrainEngine>
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
        public TrainEngineState EngineState { get; private set; }

        /// <summary>
        /// Current train speed type.
        /// </summary>
        public TrainEngineSpeedTypes CurrentEngineSpeedType { get; private set; }

        /// <summary>
        /// Current train speed type.
        /// </summary>
        public TrainEngineSpeedTypes CurrentTargetEngineSpeedType { get; private set; }

        /// <summary>
        /// Current train velocity speed.
        /// </summary>
        public float CurrentVelocity => _currentSpeed / _trainController.SplineLenght;

        /// <summary>
        /// Determines the chosen track for the next possible rail split.<br></br>
        /// <br>False = left.</br>
        /// <br>True = right.</br>
        /// </summary>
        public bool CurrentSplitDirection { get; set; }

        private Dictionary<TrainEngineSpeedTypes, float> _getSpeedValue;

        private float _currentSpeed = 0f;
        private float _speedTypesCount = 0;

        private Tween _speedTween;
        private TrainController _trainController;

        private void Awake() => _trainController = GetComponent<TrainController>();

        private void Start()
        {
            _speedTypesCount = Enum.GetValues(typeof(TrainEngineSpeedTypes)).Length - 1;

            _getSpeedValue = new Dictionary<TrainEngineSpeedTypes, float>()
            {
                {TrainEngineSpeedTypes.HIGH, _maxHighSpeed },
                {TrainEngineSpeedTypes.LOW, _maxLowSpeed },
                {TrainEngineSpeedTypes.MEDIUM, _maxMediumSpeed },
                
                {TrainEngineSpeedTypes.STILL, 0 },

                {TrainEngineSpeedTypes.LOW_REVERSE, -_maxLowSpeed },
                {TrainEngineSpeedTypes.MEDIUM_REVERSE, -_maxMediumSpeed },
                {TrainEngineSpeedTypes.HIGH_REVERSE, -_maxHighSpeed },
            };

            EngineState = TrainEngineState.ON_STANDBY;
            CurrentEngineSpeedType = TrainEngineSpeedTypes.STILL;
            CurrentTargetEngineSpeedType = TrainEngineSpeedTypes.STILL;
        }

        /// <summary>
        /// Method for increasing/decreasing train speed level.
        /// </summary>
        /// <param name="increase"></param>
        public void AdjustSpeed(bool increase)
        {
            CurrentTargetEngineSpeedType = (TrainEngineSpeedTypes)Mathf.Clamp
                    ((int)CurrentTargetEngineSpeedType + (increase ? 1 : -1), 0, _speedTypesCount);

            TweenTrainSpeed(CurrentTargetEngineSpeedType);
        }

        private void TweenTrainSpeed(TrainEngineSpeedTypes targetEngineSpeedType)
        {
            _speedTween.Kill();

            float currentMaxSpeed = _getSpeedValue[targetEngineSpeedType];
            float duration = _accelerationDuration;
            
            _speedTween = DOTween.To(() => _currentSpeed, x => _currentSpeed = x, currentMaxSpeed, duration)
                .SetEase(_accelerationEase);

            _speedTween.OnComplete(() =>
            {
                EngineState = targetEngineSpeedType == TrainEngineSpeedTypes.STILL
                    ? TrainEngineState.ON_STANDBY : TrainEngineState.ON;
                CurrentEngineSpeedType = targetEngineSpeedType;
            });

            _speedTween.Play();
        }
    }
}
