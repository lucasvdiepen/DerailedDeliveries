using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.Camera;
using FishNet.Object;

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
        private Vector3[] _stationPositions;

        private float _distance;
        private TrainController _trainController;
       
        private void Awake()
        {
            _trainController = GetComponent<TrainController>();
        }

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                TryParkTrainAtClosestStation();
        }


        [ServerRpc(RequireOwnership = false)]
        public void TryParkTrainAtClosestStation()
        {
            if (!CanParkTrain())
            {
                print("Park denied");
                return;
            }

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

            _trainController.TrainEngine.ToggleEngineState();
            CinemachineVirtualCamera nearestStationCamera = CameraManager.Instance.StationCameras[nearestStationCameraIndex];

            CameraManager.Instance.ChangeActiveCamera(nearestStationCamera);
            Animator anim = nearestStationCamera.transform.parent.GetComponent<Animator>();

            anim.SetTrigger("Enter");
        }
       
        [Server]
        private bool CanParkTrain()
        {
            return TrainEngine.Instance.CurrentSpeedIndex == 0 
                && Mathf.Abs(TrainEngine.Instance.CurrentSpeed) < .1f;
        }
    }
}
