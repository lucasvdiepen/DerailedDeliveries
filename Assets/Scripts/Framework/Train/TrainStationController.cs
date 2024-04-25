using FishNet.Object;
using Cinemachine;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.Train;
using DerailedDeliveries.Framework.Gameplay.Level;

namespace DerailedDeliveries.Framework.Camera
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

        private bool _isParked;

        /// <summary>
        /// Invoked when train <see cref="_isParked"/> state is changed.
        /// </summary>
        public Action<bool> OnParkStateChanged { get; private set; }

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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartClient()
        {
            if (!IsServer)
                return;

            Vector3 trainPosition = _trainController.Spline.EvaluatePosition(_trainController.DistanceAlongSpline);
            int nearestStationIndex = StationManager.Instance.GetNearestStationIndex(trainPosition, out _);

            ParkTrainAtClosestStation(nearestStationIndex);
        }

        private void Update()
        {
            if (!IsServer || TrainEngine.Instance.EngineState == TrainEngineState.Inactive)
                return;

            _canPark = ParkCheck(out int nearestStationIndex);

            if(!_canPark && _isParked)
            {
                UnparkTrain();
                return;
            }

            if (Mathf.Abs(TrainEngine.Instance.CurrentSpeed) <= 0.005f && !_isParked)
            {
                if (TrainEngine.Instance.CurrentGearIndex != 0 || !_canPark)
                    return;

                ParkTrainAtClosestStation(nearestStationIndex);
            }

            else if (Mathf.Abs(TrainEngine.Instance.CurrentSpeed) >= _minTrainSpeedToPark && _isParked)
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

        [ServerRpc(RequireOwnership = false)]
        private void UnparkTrain() => UnparkTrainObserver();


        [ServerRpc(RequireOwnership = false)]
        private void ParkTrainAtClosestStation(int nearestStationIndex) => ParkTrain(nearestStationIndex);

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void ParkTrain(int closestStationIndex)
        {
            SetIsParked(true);

            CinemachineVirtualCamera nearestStationCamera 
                = StationManager.Instance.StationContainers[closestStationIndex].StationCamera;
            
            CameraManager.Instance.ChangeActiveCamera(nearestStationCamera);
            
            _currentStationAnimator = nearestStationCamera.transform.parent.GetComponent<Animator>();
            _currentStationAnimator.SetTrigger(_enterAnimationHash);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void UnparkTrainObserver()
        {
            SetIsParked(false);

            if (_currentStationAnimator != null )
                _currentStationAnimator.SetTrigger(_exitAnimationHash);

            CameraManager.Instance.ChangeActiveCamera(CameraManager.Instance.TrainCamera);
            _currentStationAnimator = null;
        }

        private void SetIsParked(bool state)
        {
            print("eiheihoehoni");
            _isParked = state;
            OnParkStateChanged?.Invoke(state);
        }
    }
}
