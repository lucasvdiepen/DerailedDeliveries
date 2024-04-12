using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

using DerailedDeliveries.Framework.Utils;
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

        private TrainController _trainController;
       
        private void Awake()
        {
            _trainController = GetComponent<TrainController>();
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
            return TrainEngine.Instance.CurrentSpeedIndex != 0 
                && Mathf.Abs(TrainEngine.Instance.CurrentSpeed) > .1f;
        }

        private void TryParkTrain()
        {
            if (!CanParkTrain())
            {
                print("Park denied");
                return;
            }

            CinemachineVirtualCamera trainCamera = CameraManager.Instance.TrainCamera;

            Vector3 trainPosition = _trainController.Spline.EvaluatePosition(_trainController.DistanceAlongSpline);
            CinemachineVirtualCamera nearestCamera = CameraManager.Instance.GetNearestCamera(trainPosition, out float distance, trainCamera);

            if (distance > _minRangeToNearestStation)
                return;

            _trainController.TrainEngine.ToggleEngineState();

            CameraManager.Instance.ChangeActiveCamera(nearestCamera);
            Animator anim = nearestCamera.transform.parent.GetComponent<Animator>();

            anim.SetTrigger("Enter");
        }
    }
}
