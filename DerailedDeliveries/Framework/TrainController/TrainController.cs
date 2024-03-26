using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine;
using DG.Tweening;
using System;

namespace DerailedDeliveries.Framework.TrainController
{
    /// <summary>
    /// Class responsible for moving train along rails spline.
    /// </summary>
    public class TrainController : MonoBehaviour
    {
        [SerializeField]
        private SplineContainer _spline;
        
        [SerializeField, Range(0, 1)]
        private float _trainFrontStartTime = 0f;

        [Header("Train Config")]
        [SerializeField]
        private float _maxHighSpeed = 7.5f;
        
        [SerializeField]
        private float _maxMediumSpeed = 5f;
        
        [SerializeField]
        private float _maxLowSpeed = 2.5f;

        [SerializeField]
        private float _heightOffset = 1.0f;
        
        [SerializeField, Space]
        private float _accelerationDuration = 10f;

        [SerializeField]
        private Ease _accelerationEase = Ease.Linear;

        [Header("Wagons Config")]
        [SerializeField]
        private Transform _frontWagon = null;
        
        [SerializeField]
        private float _wagonFollowDistance = 0f;

        [SerializeField]
        private float _wagonSpacing = 0f;

        [SerializeField]
        private Transform[] _wagons = null;

        /// <summary>
        /// Current train velocity speed.
        /// </summary>
        public float CurrentVelocity => _currentSpeed / 100f;

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
        /// Current distance value along spline lenght clamped between 0-1 (same as time). <br/>
        /// <br/> 0 = Spline start point.<br/>
        /// 1 = Spline end point.<br/>
        /// </summary>
        public float DistanceAlongSpline { get; private set; } = 0.0f;

        /// <summary>
        /// Action that gets invoked when train speed type has changed.<br/>
        /// <br>TrainEngineSpeedTypes = last value.</br>
        /// <br/>TrainEngineSpeedTypes = current value.
        /// </summary>
        public Action<TrainEngineSpeedTypes, TrainEngineSpeedTypes> onSpeedTypeChanged = null;

        private Dictionary<TrainEngineSpeedTypes, float> _getSpeedValue;

        private float _currentSpeed = 0f;
        private float _speedTypesCount = 0;
        private Tween _speedTween;

        private void Start()
        {
            DistanceAlongSpline = _trainFrontStartTime;
            _speedTypesCount = Enum.GetValues(typeof(TrainEngineSpeedTypes)).Length - 1;

            //Initialize speed values dictionary.
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

        private void OnValidate()
        {
            if (!gameObject.activeSelf || Application.isPlaying)
                return;

            DebugSnapToSpline();
        }

        private void Update()
        {
            MoveTrain();

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                AdjustSpeed(true);
            }

            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                AdjustSpeed(false);
            }
        }

        private void AdjustSpeed(bool increase)
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

        private void MoveTrain()
        {
            UpdateWagonPosition(_frontWagon);

            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
            {
                float adjustedFollowDistance = _wagonFollowDistance / 10f;
                float offset = adjustedFollowDistance + (-_wagonSpacing / 10f) * i;

                UpdateWagonPosition(_wagons[i - 1], offset);
            }

            DistanceAlongSpline += CurrentVelocity * Time.deltaTime;
            if (DistanceAlongSpline > 1.0f)
                DistanceAlongSpline = 0.0f;
        }

        private void TweenTrainSpeed(TrainEngineSpeedTypes targetEngineSpeedType, bool isAccelerating)
        {
            _speedTween.Kill();

            float currentMaxSpeed = _getSpeedValue[targetEngineSpeedType];

            _speedTween = DOTween.To(()
                => _currentSpeed, x => _currentSpeed = x, currentMaxSpeed, _accelerationDuration);

            _speedTween.OnStart(() => EngineState = 
                isAccelerating ? TrainEngineState.ACCELERATING : TrainEngineState.DECELERATING);

            _speedTween.OnComplete(() =>
            {
                EngineState = targetEngineSpeedType == TrainEngineSpeedTypes.STILL 
                    ? TrainEngineState.ON_STANDBY : TrainEngineState.FULL_POWER;
                CurrentEngineSpeedType = targetEngineSpeedType;
            });

            _speedTween.Play();
        }

        /// <summary>
        /// Sets a wagon transform to the correct spline position and rotation.
        /// </summary>
        /// <param name="trainBody"></param>
        /// <param name="offset"></param>
        public void UpdateWagonPosition(Transform trainBody, float offset = 0)
        {
            Vector3 nextPosition = _spline.EvaluatePosition(DistanceAlongSpline + (offset / 10f));
            nextPosition.y += _heightOffset;
            trainBody.position = nextPosition;

            Vector3 nextDirection = _spline.EvaluateTangent(DistanceAlongSpline + (offset / 10f));
            trainBody.rotation = Quaternion.LookRotation(-nextDirection, Vector3.up);
        }

        private void DebugSnapToSpline()
        {
            DistanceAlongSpline = _trainFrontStartTime;
            UpdateWagonPosition(_frontWagon);

            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
            {
                float adjustedFollowDistance = _wagonFollowDistance / 10f;
                float offset = adjustedFollowDistance + (-_wagonSpacing / 10f) * i;
                UpdateWagonPosition(_wagons[i - 1], offset);
            }
        }
    }
}