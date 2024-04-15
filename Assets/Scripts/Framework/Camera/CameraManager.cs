using Cinemachine;
using UnityEngine;

using DerailedDeliveries.Framework.Utils;

namespace DerailedDeliveries.Framework.Camera
{
    /// <summary>
    /// Class responsible for controlling and switching between virtual cameras.
    /// </summary>
    public class CameraManager : AbstractSingleton<CameraManager>
    {
        [SerializeField]
        private CinemachineVirtualCamera[] _stationCameras;

        [field: SerializeField]
        public CinemachineVirtualCamera TrainCamera { get; private set; }

        /// <summary>
        /// Getter for all available station cameras.
        /// </summary>
        public CinemachineVirtualCamera[] StationCameras => _stationCameras;


        /// <summary>
        /// Method to change priority to a specific camera, disables all other cameras.
        /// </summary>
        /// <param name="targetCamera"></param>
        public void ChangeActiveCamera(CinemachineVirtualCamera targetCamera)
        {
            int cameras = _stationCameras.Length;

            for (int i = 0; i < cameras; i++)
            {
                _stationCameras[i].Priority = 0;
            }

            TrainCamera.Priority = 0;
            targetCamera.Priority = 1;
        }
    }
}