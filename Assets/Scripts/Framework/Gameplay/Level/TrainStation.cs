using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.UI.TextUpdaters;
using System;

namespace DerailedDeliveries.Framework.Gameplay.Level
{
    /// <summary>
    /// A class that is responsible for holding values of a Train Station.
    /// </summary>
    public class TrainStation : NetworkBehaviour
    {
        [SerializeField]
        private StationTextUpdater _labelTextUpdater;

        [SerializeField]
        private Transform[] _spawnTransforms;

        /// <summary>
        /// A getter that returns this Station's ID.
        /// </summary>
        public int StationID { get; private set; } = -1;

        /// <summary>
        /// A getter that returns this Station's Label.
        /// </summary>
        public string StationLabel { get; private set; }

        /// <summary>
        /// A getter that returns the SpawnTransforms of this station.
        /// </summary>
        public Transform[] SpawnTransforms => _spawnTransforms;

        /// <summary>
        /// An action that broadcasts the newly set <see cref="StationLabel"/>.
        /// </summary>
        public Action<string> OnStationLabelChange;

        /// <summary>
        /// Updates the stations Label and ID.
        /// </summary>
        /// <param name="label">The new label to assign.</param>
        /// <param name="id">The new ID to assign.</param>
        [ObserversRpc(RunLocally = true, BufferLast = true)]
        public void UpdateLabelAndID(string label, int id)
        {
            StationLabel = label;
            StationID = id;

            if(_labelTextUpdater != null)
                _labelTextUpdater.ReplaceTag(StationLabel);
        }
    }
}