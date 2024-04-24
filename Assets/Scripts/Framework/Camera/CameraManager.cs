using Cinemachine;
using UnityEngine;
using System.Linq;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.PlayerManagement;
using DerailedDeliveries.Framework.TrainStation;

namespace DerailedDeliveries.Framework.Camera
{
    /// <summary>
    /// Class responsible for controlling and switching between virtual cameras.
    /// </summary>
    public class CameraManager : AbstractSingleton<CameraManager>
    {
        /// <summary>
        /// Reference to the train camera.
        /// </summary>
        [field: SerializeField]
        public CinemachineVirtualCamera TrainCamera { get; private set; }

        [SerializeField]
        private CinemachineTargetGroup _targetGroup;

        private CinemachineVirtualCamera[] _stationCameras;
      
        private void OnEnable() => PlayerManager.Instance.OnPlayerJoined += HandlePlayerJoined;

        private void OnDisable() => PlayerManager.Instance.OnPlayerJoined -= HandlePlayerJoined;

        private void HandlePlayerJoined(PlayerId playerId) => _targetGroup.AddMember(playerId.transform, 1, 1);

        private void Start()
        {
            _stationCameras = StationManager.Instance.StationContainers
                .Select(stationContainer => stationContainer.StationCamera).ToArray();
        }

        /// <summary>
        /// Method for setting a specific camera active by changing the priority, disables all other cameras.
        /// </summary>
        /// <param name="targetCamera">Camera to set active.</param>
        public void ChangeActiveCamera(CinemachineVirtualCamera targetCamera)
        {
            int camerasAmount = _stationCameras.Length;
            for (int i = 0; i < camerasAmount; i++)
                _stationCameras[i].Priority = 0;

            TrainCamera.Priority = 0;
            targetCamera.Priority = 1;
        }
    }
}