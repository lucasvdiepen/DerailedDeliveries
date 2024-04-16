using FishNet.Object;
using Cinemachine;
using UnityEngine;

using DerailedDeliveries.Framework.Camera;
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

        /// <summary>
        /// Getter for when the train is parked at a station.
        /// </summary>
        public bool IsParked { get; private set; }

        private float _distance;
        private TrainController _trainController;
        private Animator _currentStationAnimator;

        private int _enterAnimationHash;
        private int _exitAnimationHash;

        private void Awake()
            => _trainController = GetComponent<TrainController>();

        private void Start()
        {
            _enterAnimationHash = Animator.StringToHash("Enter");
            _exitAnimationHash = Animator.StringToHash("Exit");
        }

        private void OnEnable()
            => TrainEngine.Instance.OnSpeedStateChanged += HandleSpeedStateChanged;

        private void OnDisable()
            => TrainEngine.Instance.OnSpeedStateChanged -= HandleSpeedStateChanged;

        private void HandleSpeedStateChanged(int newSpeedState)
        {
            if (newSpeedState == 0)
                TryParkTrainAtClosestStation();
            else if (newSpeedState > 0)
            {
                if (!IsParked)
                    return;

                IsParked = false;

                _currentStationAnimator.SetTrigger(_exitAnimationHash);
                CameraManager.Instance.ChangeActiveCamera(CameraManager.Instance.TrainCamera);
                
                _currentStationAnimator = null;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void TryParkTrainAtClosestStation()
        {
            Vector3 trainPosition = _trainController.Spline.EvaluatePosition(_trainController.DistanceAlongSpline);
            int nearestCameraIndex = CameraManager.Instance.GetNearestCamera(trainPosition, out _distance);

            if (_distance > _minRangeToNearestStation)
                return;

            TryParkTrain(nearestCameraIndex);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void TryParkTrain(int nearestStationCameraIndex)
        {
            TrainEngine.Instance.ToggleEngineState();
            CinemachineVirtualCamera nearestStationCamera = CameraManager.Instance.StationCameras[nearestStationCameraIndex];

            CameraManager.Instance.ChangeActiveCamera(nearestStationCamera);
            _currentStationAnimator = nearestStationCamera.transform.parent.GetComponent<Animator>();

            _currentStationAnimator.SetTrigger(_enterAnimationHash);
            IsParked = true;
        }
    }
}
