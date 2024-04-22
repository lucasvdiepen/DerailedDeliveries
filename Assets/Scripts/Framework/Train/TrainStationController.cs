using FishNet.Object;
using Cinemachine;
using UnityEngine;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.Train;

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

        /// <summary>
        /// Getter for when the train is parked at a station.
        /// </summary>
        public bool IsParked { get; private set; }

        private bool isTransitioning;

        private const float UNPARK_TOLERANCE = .05f;

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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartClient()
        {
            if (IsServer)
                TryParkTrainAtClosestStation();
        }

        private void OnEnable()
            => TrainEngine.Instance.OnSpeedChanged += HandleSpeedChanged;

        private void OnDisable()
            => TrainEngine.Instance.OnSpeedChanged -= HandleSpeedChanged;

        private void HandleSpeedChanged(float newSpeed)
        {
            if (!IsServer)
                return;
            
            if (newSpeed == 0 && !IsParked && !isTransitioning)
            {
                isTransitioning = true;
                TryParkTrainAtClosestStation();
            }

            else if (Mathf.Abs(newSpeed) >= _minTrainSpeedToPark && IsParked && !isTransitioning)
            {
                isTransitioning = true;
                TryUnparkTrain();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void TryUnparkTrain() => UnparkTrain();

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
            IsParked = true;
            
            CinemachineVirtualCamera nearestStationCamera = CameraManager.Instance.StationCameras[nearestStationCameraIndex];

            CameraManager.Instance.ChangeActiveCamera(nearestStationCamera);
            _currentStationAnimator = nearestStationCamera.transform.parent.GetComponent<Animator>();

            _currentStationAnimator.SetTrigger(_enterAnimationHash);

            isTransitioning = false;
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void UnparkTrain()
        {
            IsParked = false;

            if(_currentStationAnimator != null )
                _currentStationAnimator.SetTrigger(_exitAnimationHash);

            CameraManager.Instance.ChangeActiveCamera(CameraManager.Instance.TrainCamera);
            _currentStationAnimator = null;

            isTransitioning = false;
        }
    }
}
