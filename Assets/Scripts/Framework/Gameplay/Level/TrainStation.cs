using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Level
{
    /// <summary>
    /// A class that is responsible for holding values of a Train Station.
    /// </summary>
    public class TrainStation : MonoBehaviour
    {
        [SerializeField]
        private int _stationID = -1;

        [SerializeField]
        private string _stationLabel;

        [SerializeField]
        private Transform[] _spawnTransforms;

        /// <summary>
        /// A getter that returns this Station's ID.
        /// </summary>
        public int StationID => _stationID;

        /// <summary>
        /// A getter that returns this Station's Label.
        /// </summary>
        public string StationLabel => _stationLabel;

        /// <summary>
        /// A getter that returns the SpawnTransforms of this station.
        /// </summary>
        public Transform[] SpawnTransforms => _spawnTransforms;

        public int UpdateLabelAndReturnID(string label)
        {
            _stationLabel = label;
            return _stationID;
        }
    }
}