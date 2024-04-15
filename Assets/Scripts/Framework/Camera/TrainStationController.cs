using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.Camera;
using FishNet.Object;
using System;
using DG.Tweening;

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

        [field: SerializeField]
        public bool IsParked { get; private set; }

        private float _distance;
        private TrainController _trainController;
        private Animator _currentStationAnimator;

        private void Awake()
        {
            _trainController = GetComponent<TrainController>();
        }

        private void OnEnable()
        {
            TrainEngine.Instance.OnSpeedStateChanged += HandleSpeedStateChanged;
        }

        private void OnDisable()
        {
            TrainEngine.Instance.OnSpeedStateChanged -= HandleSpeedStateChanged;
        }

        private void HandleSpeedStateChanged(int newSpeedState)
        {
            if (newSpeedState == 0)
                TryParkTrainAtClosestStation();
            else if (newSpeedState > 0)
            {
                if (!IsParked)
                    return;

                IsParked = false;
                _currentStationAnimator.SetTrigger("Exit");
                CameraManager.Instance.ChangeActiveCamera(CameraManager.Instance.TrainCamera);
                //DOVirtual.Float(0, 1, .4f, null).OnComplete(() => CameraManager.Instance.ChangeActiveCamera(CameraManager.Instance.TrainCamera));
                _currentStationAnimator = null;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void TryParkTrainAtClosestStation()
        {
            print("TryParkTrainAtClosestStation");

            Vector3 trainPosition = _trainController.Spline.EvaluatePosition(_trainController.DistanceAlongSpline);
            int nearestCameraIndex = CameraManager.Instance.GetNearestCamera(trainPosition, out _distance);

            if (_distance > _minRangeToNearestStation)
                return;

            TryParkTrain(nearestCameraIndex);
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void TryParkTrain(int nearestStationCameraIndex)
        {
            print("Observer camera: " + nearestStationCameraIndex);
            TrainEngine.Instance.ToggleEngineState();

            _trainController.TrainEngine.ToggleEngineState();
            CinemachineVirtualCamera nearestStationCamera = CameraManager.Instance.StationCameras[nearestStationCameraIndex];

            CameraManager.Instance.ChangeActiveCamera(nearestStationCamera);
            _currentStationAnimator = nearestStationCamera.transform.parent.GetComponent<Animator>();

            _currentStationAnimator.SetTrigger("Enter");
            IsParked = true;
        }
    }
}
