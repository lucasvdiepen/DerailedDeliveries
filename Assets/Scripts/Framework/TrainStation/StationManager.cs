using UnityEngine;
using System;

using DerailedDeliveries.Framework.Utils;

namespace DerailedDeliveries.Framework.Gameplay.Level
{
    /// <summary>
    /// Class responsible for holding all available tran stations for easy access.
    /// </summary>
    public class StationManager : AbstractSingleton<StationManager>
    {
        /// <summary>
        /// Getter for all available train stations.
        /// </summary>
        [field: SerializeField]
        public StationCameraBlendingContainer[] StationContainers { get; private set; }

        /// <summary>
        /// Helper method responsible for getting the nearest virtual camera based on an origin position.
        /// </summary>
        /// <param name="originPosition">Position from which to look from.</param>
        /// <param name="distance">Distance to the nearest camera.</param>
        /// <param name="ignore">Cameras to ignore in search.</param>
        /// <returns>Index of nearest camera.</returns>
        public int GetNearestStationIndex(Vector3 originPosition, out float distance)
        {
            StationCameraBlendingContainer bestTarget = null;
            float closestDistanceSquare = Mathf.Infinity;

            foreach (StationCameraBlendingContainer stationContainer in StationContainers)
            {
                Vector3 directionToTarget = stationContainer.StationCamera.transform.position - originPosition;
                float distanceSquared = directionToTarget.sqrMagnitude;

                if (distanceSquared > closestDistanceSquare)
                    continue;

                closestDistanceSquare = distanceSquared;
                bestTarget = stationContainer;
            }

            distance = Mathf.Sqrt(closestDistanceSquare);
            return Array.IndexOf(StationContainers, bestTarget);
        }
    }
}
