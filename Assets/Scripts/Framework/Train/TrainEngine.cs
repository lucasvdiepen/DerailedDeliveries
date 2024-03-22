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
        /// Action that gets invoked when train speed type has changed.<br/>
        /// <br>TrainEngineSpeedTypes = last value.</br>
        /// <br/>TrainEngineSpeedTypes = current value.
        /// </summary>
        public Action<TrainEngineSpeedTypes, TrainEngineSpeedTypes> onSpeedTypeChanged = null;

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
        public float CurrentVelocity => _currentSpeed / 100f;

        private Dictionary<TrainEngineSpeedTypes, float> _getSpeedValue;

        private float _currentSpeed = 0f;
        private float _speedTypesCount = 0;

        private Tween _speedTween;

        private void Start()
        {
            _speedTypesCount = Enum.GetValues(typeof(TrainEngineSpeedTypes)).Length - 1;

            _getSpeedValue = new Dictionary<TrainEngineSpeedTypes, float>()
            {
                {TrainEngineSpeedTypes.LOW, _maxLowSpeed },
                {TrainEngineSpeedTypes.MEDIUM, _maxMediumSpeed },
                {TrainEngineSpeedTypes.HIGH, _maxHighSpeed },
                {TrainEngineSpeedTypes.STILL, 0 },
            };

            EngineState = TrainEngineState.ON_STANDBY;
            CurrentEngineSpeedType = TrainEngineSpeedTypes.STILL;
        }

        public void AdjustSpeed(bool increase)
        {
            TrainEngineSpeedTypes lastType = CurrentEngineSpeedType;

            CurrentTargetEngineSpeedType = (TrainEngineSpeedTypes)Mathf.Clamp
                    ((int)CurrentTargetEngineSpeedType + (increase ? 1 : -1), 0, _speedTypesCount);

            bool isAccelerating = (int)lastType < (int)CurrentTargetEngineSpeedType;
            TweenTrainSpeed(CurrentTargetEngineSpeedType, isAccelerating);

            //TODO: fix pls
            if (lastType != CurrentEngineSpeedType)
            {
                onSpeedTypeChanged?.Invoke(lastType, CurrentEngineSpeedType);
            }
        }

        private void TweenTrainSpeed(TrainEngineSpeedTypes targetEngineSpeedType, bool isAccelerating)
        {
            _speedTween.Kill();

            float currentMaxSpeed = _getSpeedValue[targetEngineSpeedType];

            _speedTween = DOTween.To(()
                => _currentSpeed, x => _currentSpeed = x, currentMaxSpeed, _accelerationDuration);

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
