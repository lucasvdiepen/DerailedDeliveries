using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.Camera;
using System.Linq;

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

        private TrainController _trainController;

        public int NextStationID { get; private set; } = 1;

        private Vector3[] _stationPositions;
       
        private void Awake()
        {
            _trainController = GetComponent<TrainController>();
        }

        private void Start()
        {
            CinemachineVirtualCamera[] stationCameras = CameraManager.Instance.StationCameras;
            stationCameras = new CinemachineVirtualCamera[stationCameras.Length];

            _stationPositions = (Vector3[])stationCameras.Select(camera => camera.transform.position);
        }

        private void Update()
        {
            if (!IsServer)
                return;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                TryParkTrain();
        }

        private bool CanParkTrain()
        {
            return TrainEngine.Instance.CurrentSpeedIndex == 0 
                && Mathf.Abs(TrainEngine.Instance.CurrentSpeed) < .1f;
        }

        private void TryParkTrain()
        {
            print(NextStationID);
            if (!CanParkTrain())
            {
                print("Park denied");
                return;
            }

            CinemachineVirtualCamera nearestCamera = CameraManager.Instance.StationCameras[NextStationID];

            Vector3 trainPosition = _trainController.Spline.EvaluatePosition(_trainController.DistanceAlongSpline);
            float distance = Vector3.Distance(trainPosition, nearestCamera.transform.position);

            print(distance);

            if (distance > _minRangeToNearestStation)
                return;

            _trainController.TrainEngine.ToggleEngineState();

            CameraManager.Instance.ChangeActiveCamera(nearestCamera);
            Animator anim = nearestCamera.transform.parent.GetComponent<Animator>();

            anim.SetTrigger("Enter");
        }
    }
}
