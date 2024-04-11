using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.CameraController;

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
                OnVelocityChanged();
        }

        private void OnVelocityChanged()
        {
            CinemachineVirtualCamera trainCamera = CameraManager.Instance.TrainCamera;
            CinemachineVirtualCamera nearestCamera = CameraManager.Instance.GetNearestCamera(_trainController.CenterPoint.transform.position, out float distance, trainCamera);

            if (distance > _minRangeToNearestStation)
                return;

            _trainController.TrainEngine.ToggleEngineState();
            CameraManager.Instance.ChangeActiveCamera(nearestCamera);
            Animator anim = nearestCamera.transform.parent.GetComponent<Animator>();

            anim.SetTrigger("Enter");
        }
    }
}
