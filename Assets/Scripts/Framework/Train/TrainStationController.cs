using FishNet.Object;
using Cinemachine;
using UnityEngine;
using FishNet;
using System;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.Station;
using DerailedDeliveries.Framework.Camera;

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
                OnParkStateChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Invoked when train <see cref="IsParked"/> state is changed.
        /// </summary>
        public Action<bool> OnParkStateChanged { get; private set; }

        private bool _isParked;
        private bool _canPark;

        private TrainController _trainController;
        private Animator _currentStationAnimator;

        private int _enterAnimationHash;
        private int _exitAnimationHash;

        private void Awake() => _trainController = GetComponent<TrainController>();

        private void Start()
        {
            _enterAnimationHash = Animator.StringToHash("Enter");
            _exitAnimationHash = Animator.StringToHash("Exit");
        }

        private void OnEnable() => InstanceFinder.TimeManager.OnPostTick += OnPostTick;

        private void OnDisable() => InstanceFinder.TimeManager.OnPostTick -= OnPostTick;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartServer()
        {
            Vector3 trainPosition = _trainController.Spline.EvaluatePosition(_trainController.DistanceAlongSpline);
            int nearestStationIndex = StationManager.Instance.GetNearestStationIndex(trainPosition, out _);

            ParkTrain(nearestStationIndex);
        }

        private void OnPostTick()
        {
            if (!IsServer || TrainEngine.Instance.EngineState == TrainEngineState.Inactive)
                return;

            _canPark = ParkCheck(out int nearestStationIndex);

            if(!_canPark && IsParked)
            {
                UnparkTrain();
                return;
            }

            if (Mathf.Abs(TrainEngine.Instance.CurrentSpeed) <= 0.005f && !IsParked)
            {
                if (TrainEngine.Instance.CurrentGearIndex != 0 || !_canPark)
                    return;

                ParkTrain(nearestStationIndex);
            }

            else if (Mathf.Abs(TrainEngine.Instance.CurrentSpeed) >= _minTrainSpeedToPark && IsParked)
                UnparkTrain();
        }

        [Server]
        private bool ParkCheck(out int nearestStationIndex)
        {
            Vector3 trainPosition = _trainController.Spline.EvaluatePosition(_trainController.DistanceAlongSpline);
            nearestStationIndex = StationManager.Instance.GetNearestStationIndex(trainPosition, out _);

            StationCameraBlendingContainer closestStation = StationManager.Instance.StationContainers[nearestStationIndex];

            bool min = closestStation.StationBoundingBoxCollider.bounds.Contains(_minimumPoint.position);
            bool max = closestStation.StationBoundingBoxCollider.bounds.Contains(_maximumPoint.position);

            return min && max;
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void ParkTrain(int closestStationIndex)
        {
            IsParked = true;

            CinemachineVirtualCamera nearestStationCamera 
                = StationManager.Instance.StationContainers[closestStationIndex].StationCamera;
            
            CameraManager.Instance.ChangeActiveCamera(nearestStationCamera);
            
            _currentStationAnimator = nearestStationCamera.transform.parent.GetComponent<Animator>();
            _currentStationAnimator.SetTrigger(_enterAnimationHash);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void UnparkTrain()
        {
            IsParked = false;

            if (_currentStationAnimator != null )
                _currentStationAnimator.SetTrigger(_exitAnimationHash);

            CameraManager.Instance.ChangeActiveCamera(CameraManager.Instance.TrainCamera);
            _currentStationAnimator = null;
        }
    }
}
