using UnityEngine.Playables;
using UnityEngine;
using System.Linq;

using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.Utils.Vector3Extentions;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for keeping track of all stations.
    /// </summary>
    public class StationManager : AbstractSingleton<StationManager>
    {
        [SerializeField]
        private PlayableDirector[] _stationDirectors;

        /// <summary>
        /// Helper method responsible for getting the nearest station director based on an origin position.
        /// </summary>
        /// <param name="originPosition">Position from which to look from.</param>
        /// <returns>Nearest position, if null return infinity.</returns>
        public Vector3 GetNearestStationDirector(Vector3 originPosition)
        {
            Vector3[] positions = _stationDirectors.Select(director => director.transform.position).ToArray();
            return Vector3Extentions.GetNearest(originPosition, positions);
        }
    }
}
