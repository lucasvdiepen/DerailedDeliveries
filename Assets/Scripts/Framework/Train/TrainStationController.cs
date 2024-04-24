using FishNet.Object;
using Cinemachine;
using UnityEngine;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.Train;
using UnityEngine.InputSystem;
using DerailedDeliveries.Framework.TrainStation;

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

        [SerializeField]
        private float _minimumDistanceToPark;

        [SerializeField]
        private float _maximumDistanceToPark;

        [SerializeField]
        private Transform _minimumPoint;

        [SerializeField]
        private Transform _maximumPoint;

        /// <summary>
        /// Getter for when the train is parked at a station.
        /// </summary>
        public bool IsParked { get; private set; }

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
            if (IsServer)
                TryParkTrainAtClosestStation();
        }

        private void Update()
        {
            if (!IsServer || TrainEngine.Instance.EngineState == TrainEngineState.Inactive)
                return;

            if (Mathf.Abs(TrainEngine.Instance.CurrentSpeed) <= _minTrainSpeedToPark && !IsParked)
                TryParkTrainAtClosestStation();

            else if (Mathf.Abs(TrainEngine.Instance.CurrentSpeed) >= _minTrainSpeedToPark && IsParked)
                UnparkTrain();
        }

        [ServerRpc(RequireOwnership = false)]
        private void UnparkTrain() => UnparkTrainObserver();


        [ServerRpc(RequireOwnership = false)]
        private void TryParkTrainAtClosestStation()
        {
            Vector3 trainPosition = _trainController.Spline.EvaluatePosition(_trainController.DistanceAlongSpline);
            int nearestStationIndex = StationManager.Instance.GetNearestStationIndex(trainPosition, out float _distance);

            StationContainer closestStation = StationManager.Instance.StationContainers[nearestStationIndex];

            Vector3 minPosition = _minimumPoint.transform.position;
            Vector3 maxPosition = _maximumPoint.transform.position;

            float minDist = Vector2.Distance(minPosition, closestStation.LeftCornerPoint.position);
            float maxDist = Vector2.Distance(maxPosition, closestStation.RightCornerPoint.position);

            if (_distance > _minRangeToNearestStation)
                return;

            TryParkTrain(nearestStationIndex);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void TryParkTrain(int closestStationIndex)
        {
            IsParked = true;

            CinemachineVirtualCamera nearestStationCamera 
                = StationManager.Instance.StationContainers[closestStationIndex].StationCamera;
            
            CameraManager.Instance.ChangeActiveCamera(nearestStationCamera);
            
            _currentStationAnimator = nearestStationCamera.transform.parent.GetComponent<Animator>();
            _currentStationAnimator.SetTrigger(_enterAnimationHash);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void UnparkTrainObserver()
        {
            IsParked = false;

            if(_currentStationAnimator != null )
                _currentStationAnimator.SetTrigger(_exitAnimationHash);

            CameraManager.Instance.ChangeActiveCamera(CameraManager.Instance.TrainCamera);
            _currentStationAnimator = null;
        }
    }
}
