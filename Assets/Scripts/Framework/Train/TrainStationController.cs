using FishNet.Object;
using UnityEngine;
using FishNet;
using System;

using DerailedDeliveries.Framework.Station;
using DerailedDeliveries.Framework.Utils;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for stopping train and opening doors by stations.
    /// </summary>
    [RequireComponent(typeof(TrainController))]
    public class TrainStationController : NetworkAbstractSingleton<TrainStationController>
    {
        [SerializeField]
        private float _minRangeToNearestStation = 25;

        [SerializeField]
        private float _minTrainSpeedToPark = 0.35f;

        [SerializeField, Space]
        private Transform _minimumPoint;

        [SerializeField]
        private Transform _maximumPoint;

        /// <summary>
        /// Getter for when train is parked.
        /// </summary>
        public bool IsParked
        {
            get => _isParked;
            set 
            {
                _isParked = value;
                OnParkStateChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Invoked when train <see cref="IsParked"/> state is changed.
        /// </summary>
        public Action<bool> OnParkStateChanged;

        private bool _isParked = true;
        private bool _canPark;

        private TrainController _trainController;

        private void Awake() => _trainController = GetComponent<TrainController>();

        /// <summary>
        /// Temporary disabled.
        /// </summary>
        private void OnEnable() => InstanceFinder.TimeManager.OnPostTick += OnPostTick;

        /// <summary>
        /// Temporary disabled.
        /// </summary>
        private void OnDisable()
        {
            if (InstanceFinder.TimeManager != null)
                InstanceFinder.TimeManager.OnPostTick -= OnPostTick;
        }

        private void OnPostTick()
        {
            if (!IsServer || TrainEngine.Instance.EngineState == TrainEngineState.Inactive)
                return;

            _canPark = ParkCheck();

            if(!_canPark && IsParked)
            {
                UnparkTrain();
                return;
            }

            if (Mathf.Abs(TrainEngine.Instance.CurrentSpeed) <= 0.005f && !IsParked)
            {
                if (TrainEngine.Instance.CurrentGearIndex != 0 || !_canPark)
                    return;

                ParkTrain();
            }

            else if (Mathf.Abs(TrainEngine.Instance.CurrentSpeed) >= _minTrainSpeedToPark && IsParked)
                UnparkTrain();
        }

        [Server]
        private bool ParkCheck()
        {
            Vector3 trainPosition = _trainController.Spline.EvaluatePosition(_trainController.DistanceAlongSpline);
            int nearestStationIndex = StationManager.Instance.GetNearestStationIndex(trainPosition, out _);

            StationContainer closestStation = StationManager.Instance.StationContainers[nearestStationIndex];

            bool min = closestStation.StationBoundingBoxCollider.bounds.Contains(_minimumPoint.position);
            bool max = closestStation.StationBoundingBoxCollider.bounds.Contains(_maximumPoint.position);

            return min && max;
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void ParkTrain() => IsParked = true;

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void UnparkTrain() => IsParked = false;
    }
}
