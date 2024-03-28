using UnityEngine.Splines;
using FishNet.Object;
using UnityEngine;
using System;
using FishNet;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for moving train along rails spline.
    /// </summary>
    [RequireComponent(typeof(TrainEngine))]
    public class TrainController : NetworkBehaviour
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
        public float SplineLength { get; private set; } = 0;

        /// <summary>
        /// Reference to the train engine.
        /// </summary>
        public TrainEngine TrainEngine { get; private set; }
        
        /// <summary>
        /// Helper method for updating the current spline lenght.
        /// </summary>
        public void RecalculateSplineLenght() => SplineLength = Spline.CalculateLength();

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

        private void OnEnable()
            => InstanceFinder.TimeManager.OnTick += OnTick;

        private void OnDisable()
            => InstanceFinder.TimeManager.OnTick -= OnTick;

        private void OnTick() // Server only.
        {
            if (!IsServer)
                return;

            DistanceAlongSpline += TrainEngine.CurrentVelocity * Time.fixedDeltaTime;
            
            if (DistanceAlongSpline >= 1.0f && TrainEngine.IsTraveling())
            {
                // Check for possible upcomming rail split.
                if (_railSplit != null)
                {
                    DistanceAlongSpline = 0.0f;
                    SplineContainer nextContainer = _railSplit.PossibleTracks[TrainEngine.CurrentSplitDirection ? 1 : 0];

                    int nextTrackID = SplineManager.Instance.GetIDByTrack(nextContainer);

                    // Switch current track to the new track.
                    SwitchCurrentTrack(nextTrackID, true);
                }
                else
                {
                    print("End reached");
                }
            }

            if (DistanceAlongSpline <= CurrentOptimalStartPoint && TrainEngine.IsTravelingReverse())
            {
                // Check for possible backward rail split.
                if (Spline.transform.parent != null)
                {
                    DistanceAlongSpline = 1.0f;
                    SplineContainer nextContainer = Spline.transform.parent.GetComponent<SplineContainer>();
                    
                    int nextTrackID = SplineManager.Instance.GetIDByTrack(nextContainer);
                    
                    // Switch current track to the new track.
                    SwitchCurrentTrack(nextTrackID);
                }
                else
                {
                    print("Start reached");
                    DistanceAlongSpline = CurrentOptimalStartPoint;
                }
            }

            // Loop over all wagons and update them.
            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
                MoveTrain(DistanceAlongSpline);
        }

        [ObserversRpc(RunLocally = true)]
        private void MoveTrain(float distanceAlongSpline)
        {
            UpdateWagonPosition(_frontWagon, distanceAlongSpline);

            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
            {
                // Calculate appropriate spacing/offset.
                float adjustedFollowDistance = _wagonFollowDistance / TWEAK_DIVIDE_FACTOR;
                float offset = adjustedFollowDistance + (-_wagonSpacing / TWEAK_DIVIDE_FACTOR) * i;

                UpdateWagonPosition(_wagons[i - 1], distanceAlongSpline, offset / SplineLength);
            }
        }

        [ObserversRpc(RunLocally = true)]
        private void SwitchCurrentTrack(int trackID, bool setDistanceAlongSpline = false)
        {
            Spline = SplineManager.Instance.GetTrackByID(trackID);

            RecalculateSplineLenght();
            CurrentOptimalStartPoint = GetOptimalTrainStartPoint();

            if(setDistanceAlongSpline)
                DistanceAlongSpline = CurrentOptimalStartPoint;

            Spline.gameObject.TryGetComponent(out _railSplit);
        }

        /// <summary>
        /// Calculates and returns the correct start distance along the current spline based on the length of the train.
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <returns>Spline distance value.</returns>
        private float GetOptimalTrainStartPoint()
        {
            int wagons = _wagons.Length;
            
            float adjustedFollowDistance = _wagonFollowDistance / TWEAK_DIVIDE_FACTOR;
            float offset = adjustedFollowDistance + (-_wagonSpacing / TWEAK_DIVIDE_FACTOR) * wagons;

            float offsetSum = Mathf.Abs(offset / SplineLength / TWEAK_DIVIDE_FACTOR);

            return offsetSum;
        }

        /// <summary>
        /// Sets a wagon transform to the correct spline position and rotation.
        /// </summary>
        /// <param name="trainBody">Transform of affected wagon.</param>
        /// <param name="offset">Optional offset applied on <see cref="DistanceAlongSpline"/></param>
        public void UpdateWagonPosition(Transform trainBody, float distanceAlongSpline, float offset = 0)
        {
            Vector3 nextPosition = Spline.EvaluatePosition(distanceAlongSpline + (offset / TWEAK_DIVIDE_FACTOR));
            nextPosition.y += _heightOffset;
            trainBody.position = nextPosition;

            Vector3 nextDirection = Spline.EvaluateTangent(distanceAlongSpline + (offset / TWEAK_DIVIDE_FACTOR));
            trainBody.rotation = Quaternion.LookRotation(-nextDirection, Vector3.up);
        }
        
        /// <summary>
        /// Helper method for resetting train position to the current spline start point based on its length. (Editor only)
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
            CurrentOptimalStartPoint = GetOptimalTrainStartPoint();

            if (_trainFrontStartTime <= CurrentOptimalStartPoint)
                _trainFrontStartTime = CurrentOptimalStartPoint;

            DistanceAlongSpline = overrideSplinePosition == 0 ? _trainFrontStartTime : overrideSplinePosition;
            UpdateWagonPosition(_frontWagon, DistanceAlongSpline);

            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
            {
                float adjustedFollowDistance = _wagonFollowDistance / TWEAK_DIVIDE_FACTOR;
                float offset = adjustedFollowDistance + (-_wagonSpacing / TWEAK_DIVIDE_FACTOR) * i;
                
                UpdateWagonPosition(_wagons[i - 1], DistanceAlongSpline,  offset / SplineLength);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
                return;

            if (SplineLength == 0)
                SplineLength = Spline.CalculateLength();

            if (Application.isPlaying)
                return;

            DebugSnapToSpline();
        }
#endif
    }
}
