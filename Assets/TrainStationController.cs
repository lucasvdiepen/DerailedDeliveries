using Cinemachine;
using DerailedDeliveries.Framework.Utils;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
            CinemachineVirtualCamera trainCamera = CameraController.Instance.TrainCamera;
            CinemachineVirtualCamera nearestCamera = CameraController.Instance.GetNearestCamera(_trainController.CenterPoint.transform.position, out float distance, trainCamera);

            if (distance > _minRangeToNearestStation)
                return;

            _trainController.TrainEngine.ToggleEngineState();
            CameraController.Instance.ChangeActiveCamera(nearestCamera);
            Animator anim = nearestCamera.transform.parent.GetComponent<Animator>();

            anim.SetTrigger("Enter");
        }
    }
}
