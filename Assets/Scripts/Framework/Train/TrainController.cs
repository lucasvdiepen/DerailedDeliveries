using UnityEngine.Splines;
using UnityEngine;
using System;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for moving train along rails spline.
    /// </summary>
    [RequireComponent(typeof(TrainEngine))]
    public class TrainController : MonoBehaviour
    {
        /// <summary>
        /// Reference to the spline line data.
        /// </summary>
        [field: SerializeField]
        public SplineContainer Spline { get; private set; }
        
        [Header("Train Config")]
        [SerializeField, Range(0, 1)]
        private float _trainFrontStartTime = 0f;

        [SerializeField]
        private float _heightOffset = 1.0f;

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
        /// Current distance value along spline lenght clamped between 0-1 (same as time). <br/>
        /// <br/> 0 = Spline start point.<br/>
        /// 1 = Spline end point.<br/>
        /// </summary>
        public float DistanceAlongSpline
        {
            get => _distanceAlongSpline;
            set => _distanceAlongSpline = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Optimal start point for train on spline track based on its length. 
        /// </summary>
        public float CurrentOptimalStartPoint { get; private set; }

        /// <summary>
        /// Returns the precalculated line lenght of the spline
        /// </summary>
        public float SplineLenght { get; private set; } = 0;

        /// <summary>
        /// Reference to the train engine.
        /// </summary>
        public TrainEngine TrainEngine { get; private set; }
        
        /// <summary>
        /// Helper method for updating the current spline lenght.
        /// </summary>
        public void RecalculateSplineLenght() => SplineLenght = Spline.CalculateLength();

        private RailSplit _railSplit;

        private const float TWEAK_DIVIDE_FACTOR = 10;

        private float _distanceAlongSpline = 0;

        private void Awake()
        {
            TrainEngine = GetComponent<TrainEngine>();
            DistanceAlongSpline = _trainFrontStartTime;
            CurrentOptimalStartPoint = _trainFrontStartTime;

            if (Spline != null)
                RecalculateSplineLenght();

            _railSplit = Spline.gameObject.GetComponent<RailSplit>();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!gameObject.activeSelf)
                return;

            if (SplineLenght == 0)
                SplineLenght = Spline.CalculateLength();

            if (Application.isPlaying)
                return;

            DebugSnapToSpline();
        }
#endif
        private void Update() 
            => MoveTrain();

        private void MoveTrain()
        {
            UpdateWagonPosition(_frontWagon);

            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
            {
                float adjustedFollowDistance = _wagonFollowDistance / TWEAK_DIVIDE_FACTOR;
                float offset = adjustedFollowDistance + (-_wagonSpacing / TWEAK_DIVIDE_FACTOR) * i;

                UpdateWagonPosition(_wagons[i - 1], offset / SplineLenght);
            }

            // Move movement
            DistanceAlongSpline += TrainEngine.CurrentVelocity * Time.deltaTime;

            if (DistanceAlongSpline >= 1.0f && ((int)TrainEngine.CurrentEngineSpeedType > 3 || (int)TrainEngine.CurrentTargetEngineSpeedType > 3))
                HandlePossibleRailSplit();

            if(DistanceAlongSpline <= CurrentOptimalStartPoint && ((int)TrainEngine.CurrentEngineSpeedType < 3 || (int)TrainEngine.CurrentTargetEngineSpeedType < 3))
            {

                if (Spline.transform.parent == null)
                    return;

                DistanceAlongSpline = 1f;
                Spline = Spline.transform.parent.GetComponent<SplineContainer>();

                RecalculateSplineLenght();

                CurrentOptimalStartPoint = GetOptimalTrainStartPoint();
                Spline.gameObject.TryGetComponent(out _railSplit);
            }
        }

        /// <summary>
        /// Checks for possible upcomming rail splittings. If none are found, end is reached.
        /// </summary>
        private void HandlePossibleRailSplit()
        {
            if (_railSplit == null)
            {
                print("End reached");
                return;
            }

            DistanceAlongSpline = 0.0f;
            Spline = _railSplit.PossibleTracks[TrainEngine.CurrentSplitDirection ? 1 : 0];

            RecalculateSplineLenght();
            CurrentOptimalStartPoint = GetOptimalTrainStartPoint();

            _distanceAlongSpline = CurrentOptimalStartPoint;
            Spline.gameObject.TryGetComponent(out _railSplit);
        }
        
        /// <summary>
        /// Calculates and returns the correct start distance along the current spline.
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <returns>Spline distance value.</returns>
        private float GetOptimalTrainStartPoint()
        {
            int wagons = _wagons.Length;
            
            float adjustedFollowDistance = _wagonFollowDistance / TWEAK_DIVIDE_FACTOR;
            float offset = adjustedFollowDistance + (-_wagonSpacing / TWEAK_DIVIDE_FACTOR) * wagons;

            float offsetSum = Mathf.Abs(offset / SplineLenght / TWEAK_DIVIDE_FACTOR);

            
            return offsetSum;
        }

        public float GetOptimalTrainEndPoint()
        {
            return 0;
        }

        /// <summary>
        /// Sets a wagon transform to the correct spline position and rotation.
        /// </summary>
        /// <param name="trainBody">Transform of affected wagon.</param>
        /// <param name="offset">Optional offset applied on <see cref="DistanceAlongSpline"/></param>
        public void UpdateWagonPosition(Transform trainBody, float offset = 0)
        {
            Vector3 nextPosition = Spline.EvaluatePosition(DistanceAlongSpline + (offset / TWEAK_DIVIDE_FACTOR));
            nextPosition.y += _heightOffset;
            trainBody.position = nextPosition;

            Vector3 nextDirection = Spline.EvaluateTangent(DistanceAlongSpline + (offset / TWEAK_DIVIDE_FACTOR));
            trainBody.rotation = Quaternion.LookRotation(-nextDirection, Vector3.up);
        }
        
        /// <summary>
        /// Helper method for resetting train position to the current spline start point based on its length.
        /// </summary>
        public void ResetTrainPosition()
        {
            _trainFrontStartTime = GetOptimalTrainStartPoint();
            DebugSnapToSpline(_trainFrontStartTime);
        }

        /// <summary>
        /// Helper method to snap train wagons to the correct spline position/rotation for editor use only.
        /// </summary>
        public void DebugSnapToSpline(float overrideSplinePosition = 0)
        {
            DistanceAlongSpline = overrideSplinePosition == 0 ? _trainFrontStartTime : overrideSplinePosition;
            UpdateWagonPosition(_frontWagon);

            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
            {
                float adjustedFollowDistance = _wagonFollowDistance / TWEAK_DIVIDE_FACTOR;
                float offset = adjustedFollowDistance + (-_wagonSpacing / TWEAK_DIVIDE_FACTOR) * i;
                
                UpdateWagonPosition(_wagons[i - 1], offset / SplineLenght);
            }
        }
    }
}
