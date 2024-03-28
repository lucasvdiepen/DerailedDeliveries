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
        private List<SplineContainer> splineContainers = new();

        /// <summary>
        /// Helper method for getting the spline track based on a specified ID.
        /// </summary>
        /// <param name="trackID">Track ID interger.</param>
        /// <returns>Correct SplineContainer.</returns>
        public SplineContainer GetTrackByID(int trackID) => splineContainers[trackID];

        /// <summary>
        /// Helper method for getting the ID of a specified spline track.
        /// </summary>
        /// <param name="track">SplineContainer track.</param>
        /// <returns>Correct SplineContainer ID.</returns>
        public int GetIDByTrack(SplineContainer track) => splineContainers.IndexOf(track);
    }
}
