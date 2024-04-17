using Cinemachine;
using UnityEngine;
using System;

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

        /// <summary>
        /// Reference to the train camera.
        /// </summary>
        [field: SerializeField]
        public CinemachineVirtualCamera TrainCamera { get; private set; }

        /// <summary>
        /// Getter for all available station cameras.
        /// </summary>
        public CinemachineVirtualCamera[] StationCameras => _stationCameras;

        /// <summary>
        /// Helper method responsible for getting the nearest virtual camera based on an origin position.
        /// </summary>
        /// <param name="originPosition">Position from which to look from.</param>
        /// <param name="distance">Distance to the nearest camera.</param>
        /// <param name="ignore">Cameras to ignore in search.</param>
        /// <returns>Index of nearest camera.</returns>
        public int GetNearestCamera(Vector3 originPosition, out float distance, params CinemachineVirtualCamera[] ignore)
        {
            CinemachineVirtualCamera bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;

            foreach (CinemachineVirtualCamera virtualCamera in _stationCameras)
            {
                Vector3 directionToTarget = virtualCamera.transform.position - originPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    if (!CheckIgnore(ignore, virtualCamera))
                        continue;

                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = virtualCamera;
                }
            }

            distance = Mathf.Sqrt(closestDistanceSqr);
            return Array.IndexOf(_stationCameras, bestTarget);
        }

        /// <summary>
        /// Method for setting a specific camera to active using priority, disables all other cameras.
        /// </summary>
        /// <param name="targetCamera">Camera to set active.</param>
        public void ChangeActiveCamera(CinemachineVirtualCamera targetCamera)
        {
            int cameras = _stationCameras.Length;
            for (int i = 0; i < cameras; i++)
                _stationCameras[i].Priority = 0;

            TrainCamera.Priority = 0;
            targetCamera.Priority = 1;
        }

        private bool CheckIgnore(CinemachineVirtualCamera[] ignore, CinemachineVirtualCamera cameraToCheck)
        {
            for (int i = 0; i < ignore.Length; i++)
            {
                if (ignore[i] == cameraToCheck)
                    return false;
            }

            return true;
        }
    }
}