using UnityEngine.Splines;
using FishNet.Object;
using Cinemachine;
using UnityEngine;
using System;

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
        private float _trainFrontStartTime;

        [SerializeField]
        private float _heightOffset;

        [Header("Wagons Config")]
        [SerializeField]
        private Transform _frontWagon;

        [SerializeField]
        private float _wagonFollowDistance;

        [SerializeField]
        private float _wagonSpacing;

        [SerializeField]
        private Transform[] _followingWagons;

        /// <summary>
        /// Getter for getting the middle wagon.
        /// </summary>
        public Transform MiddleWagon
        {
            get
            {
                float wagons = _followingWagons.Length + 1;
                return _followingWagons[(int)Mathf.Floor(wagons / 2f)];
            }
        }

        /// <summary>
        /// Current distance value along spline length clamped between 0-1 (same as time). <br/>
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
        /// Returns the precalculated line length of the spline.
        /// </summary>
        public float SplineLength { get; private set; }

        /// <summary>
        /// Reference to the train engine.
        /// </summary>
        public TrainEngine TrainEngine { get; private set; }

        /// <summary>
        /// Helper method for updating the current spline length.
        /// </summary>
        public void RecalculateSplineLength() => SplineLength = Spline.CalculateLength();

        private RailSplit _railSplit;

        private const float TWEAK_DIVIDE_FACTOR = 10;

        private float _distanceAlongSpline;

        private void Awake()
        {
            TrainEngine = GetComponent<TrainEngine>();

            DistanceAlongSpline = _trainFrontStartTime;
            CurrentOptimalStartPoint = _trainFrontStartTime;

            if (Spline != null)
                RecalculateSplineLength();

            _railSplit = Spline.gameObject.GetComponent<RailSplit>();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsServer)
                TimeManager.OnTick += OnTick;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (IsServer)
                TimeManager.OnTick -= OnTick;
        }

        [Server]
        private void OnTick()
        {
            // Update train position along spline based on its velocity.
            DistanceAlongSpline += TrainEngine.CurrentVelocity * (float)TimeManager.TickDelta;

            CheckUpcommingRailSplit();
            CheckReverseRailSplit();

            MoveTrain(DistanceAlongSpline);
        }

        [ObserversRpc(RunLocally = true)]
        private void MoveTrain(float distanceAlongSpline)
        {
            Vector3 positionSum = Vector3.zero;
            UpdateWagonPosition(_frontWagon, distanceAlongSpline);

            positionSum += _frontWagon.position;

            int wagonsAmount = _followingWagons.Length;
            for (int i = 0; i < wagonsAmount; i++)
            {
                // Calculate appropriate spacing/offset.
                float adjustedFollowDistance = _wagonFollowDistance / TWEAK_DIVIDE_FACTOR;
                float offset = adjustedFollowDistance + (-_wagonSpacing / TWEAK_DIVIDE_FACTOR) * (i + 1);
                UpdateWagonPosition(_followingWagons[i], distanceAlongSpline, offset / SplineLength);

                positionSum += _followingWagons[i].position;
            }
        }

        /// <summary>
        /// Internally used to check for upcomming rail splits and switching tracks.
        /// </summary>
        [Server]
        private void CheckUpcommingRailSplit()
        {
            if (DistanceAlongSpline < 1.0f || TrainEngine.CurrentVelocity < 0)
                return;

            if (_railSplit == null)
                return;

            DistanceAlongSpline = 0.0f;
            SplineContainer nextContainer = _railSplit.PossibleTracks[TrainEngine.CurrentSplitDirection ? 1 : 0];

            int nextTrackID = SplineManager.Instance.GetIDByTrack(nextContainer);

            // Switch current track to the new track.
            SwitchCurrentTrack(nextTrackID, true);
        }

        /// <summary>
        /// Internally used to check for rail splits while reversing and switching tracks.
        /// </summary>
        [Server]
        private void CheckReverseRailSplit()
        {
            if (DistanceAlongSpline > CurrentOptimalStartPoint || TrainEngine.CurrentVelocity > 0)
                return;

            // Check for possible backward rail split.
            if (!Spline.transform.parent.TryGetComponent(out SplineContainer nextSplineContainer))
            {
                DistanceAlongSpline = CurrentOptimalStartPoint;
                return;
            }

            DistanceAlongSpline = 1.0f;
            int nextTrackID = SplineManager.Instance.GetIDByTrack(nextSplineContainer);

            // Switch current track to the new track.
            SwitchCurrentTrack(nextTrackID);
        }

        /// <summary>
        /// Internally used to switch train currents spline track.
        /// </summary>
        /// <param name="trackID">ID of the track.</param>
        /// <param name="setDistanceAlongSpline">Whether the train should snap to optimal starting point.</param>
        [ObserversRpc(RunLocally = true)]
        private void SwitchCurrentTrack(int trackID, bool setDistanceAlongSpline = false)
        {
            // Get correct spline by given ID.
            Spline = SplineManager.Instance.GetTrackByID(trackID);

            // Refresh spline length and optimal start point.
            RecalculateSplineLength();
            CurrentOptimalStartPoint = GetOptimalTrainStartPoint();

            if (setDistanceAlongSpline)
                DistanceAlongSpline = CurrentOptimalStartPoint;

            Spline.gameObject.TryGetComponent(out _railSplit);
        }

        /// <summary>
        /// Calculates and returns the correct start distance along the current spline based on the length of the train.
        /// </summary>
        /// <returns>Spline distance value.</returns>
        private float GetOptimalTrainStartPoint()
        {
            int wagons = _followingWagons.Length;

            float adjustedFollowDistance = _wagonFollowDistance / TWEAK_DIVIDE_FACTOR;
            float offset = adjustedFollowDistance + (-_wagonSpacing / TWEAK_DIVIDE_FACTOR) * wagons;

            float offsetSum = Mathf.Abs(offset / SplineLength / TWEAK_DIVIDE_FACTOR);

            return offsetSum;
        }

        /// <summary>
        /// Sets a wagon transform to the correct spline position and rotation.
        /// </summary>
        /// <param name="trainBody">Transform of affected wagon.</param>
        /// <param name="offset">Optional offset applied on <see cref="DistanceAlongSpline"/>.</param>
        public void UpdateWagonPosition(Transform trainBody, float distanceAlongSpline, float offset = 0)
        {
            float totalSplineTime = distanceAlongSpline + (offset / TWEAK_DIVIDE_FACTOR);

            Vector3 nextPosition = Spline.EvaluatePosition(totalSplineTime);
            nextPosition.y += _heightOffset;
            trainBody.position = nextPosition;

            Vector3 nextDirection = Spline.EvaluateTangent(totalSplineTime);
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

            DistanceAlongSpline = overrideSplinePosition == 0
                ? _trainFrontStartTime
                : overrideSplinePosition;

            UpdateWagonPosition(_frontWagon, DistanceAlongSpline);
            MoveTrain(DistanceAlongSpline);
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
