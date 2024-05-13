using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine;

using DerailedDeliveries.Framework.Utils;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for storing all splinecontainers.
    /// </summary>
    public class SplineManager : AbstractSingleton<SplineManager>
    {
        [SerializeField]
        private List<SplineContainer> _splineContainers = new();

        /// <summary>
        /// Getter for amount of available rail splits.
        /// </summary>
        [field: SerializeField]
        public int RailSplitAmount { get; set; }

        /// <summary>
        /// Helper method for getting the spline track based on a specified ID.
        /// </summary>
        /// <param name="trackID">Track ID interger.</param>
        /// <returns>Correct SplineContainer.</returns>
        public SplineContainer GetTrackByID(int trackID) => _splineContainers[trackID];

        /// <summary>
        /// Helper method for getting the ID of a specified spline track.
        /// </summary>
        /// <param name="track">SplineContainer track.</param>
        /// <returns>Correct SplineContainer ID.</returns>
        public int GetIDByTrack(SplineContainer track) => _splineContainers.IndexOf(track);

        private void Awake()
        {
            if (RailSplitAmount <= 0)
                Debug.LogError("Rail split count cannot be less or equal than zero.", this);
        }
    }
}
