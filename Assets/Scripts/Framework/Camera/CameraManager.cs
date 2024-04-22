using Cinemachine;
using UnityEngine;
using System;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.PlayerManagement;

namespace DerailedDeliveries.Framework.Camera
{
    /// <summary>
    /// Class responsible for controlling and switching between virtual cameras.
    /// </summary>
    public class CameraManager : AbstractSingleton<CameraManager>
    {
        /// <summary>
        /// Getter/Setter for all available station cameras.
        /// </summary>
        [field: SerializeField]
        public CinemachineVirtualCamera[] StationCameras { get; private set; }

        /// <summary>
        /// Reference to the train camera.
        /// </summary>
        [field: SerializeField]
        public CinemachineVirtualCamera TrainCamera { get; private set; }

        [SerializeField]
        private CinemachineTargetGroup _targetGroup;

        private void OnEnable() => PlayerManager.Instance.OnPlayerJoined += HandlePlayerJoined;

        private void OnDisable() => PlayerManager.Instance.OnPlayerJoined -= HandlePlayerJoined;

        private void HandlePlayerJoined(PlayerId playerId) => _targetGroup.AddMember(playerId.transform, 1, 1);

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
            float closestDistanceSquare = Mathf.Infinity;

            foreach (CinemachineVirtualCamera virtualCamera in StationCameras)
            {
                Vector3 directionToTarget = virtualCamera.transform.position - originPosition;
                float distanceSquared = directionToTarget.sqrMagnitude;

                if (distanceSquared > closestDistanceSquare || !CheckIgnore(ignore, virtualCamera))
                    continue;

                closestDistanceSquare = distanceSquared;
                bestTarget = virtualCamera;
            }

            distance = Mathf.Sqrt(closestDistanceSquare);
            return Array.IndexOf(StationCameras, bestTarget);
        }

        /// <summary>
        /// Method for setting a specific camera to active using priority, disables all other cameras.
        /// </summary>
        /// <param name="targetCamera">Camera to set active.</param>
        public void ChangeActiveCamera(CinemachineVirtualCamera targetCamera)
        {
            int camerasAmount = StationCameras.Length;
            for (int i = 0; i < camerasAmount; i++)
                StationCameras[i].Priority = 0;

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