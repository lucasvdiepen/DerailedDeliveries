using DerailedDeliveries.Framework.Utils;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for stopping train and opening doors by stations.
    /// </summary>
    public class TrainStationController : AbstractSingleton<TrainStationController>
    {
        [SerializeField]
        private float _minRangeToNearestStation = 100;

        /// <summary>
        /// Method used to dock a train to the nearest available station.
        /// </summary>
        /// <returns>True if there is an available station nearby.</returns>
        [ServerRpc(RequireOwnership = false)]
        public bool TryDockTrainAtNearestStation()
        {
            Vector3 possibleStationPosition = StationManager.Instance.GetNearestStationDirector(transform.position);
            return Vector3.Distance(possibleStationPosition, transform.position) > _minRangeToNearestStation;
        }
    }
}
