using System;

namespace DerailedDeliveries.Framework.Gameplay.Level
{
    /// <summary>
    /// A struct that holds level data for a station in a level.
    /// </summary>
    [Serializable]
    public struct StationLevelData
    {
        /// <summary>
        /// A bool that indicates if we want to go all in on random.
        /// </summary>
        public bool RandomizeSpawns;

        /// <summary>
        /// The StationID of the station this data is for.
        /// </summary>
        public int StationID;

        /// <summary>
        /// The minimum amount of deliverable packages to spawn on this station.
        /// </summary>
        public int MinDeliverablePackages;

        /// <summary>
        /// The maximum amount of packages to spawn on this station.
        /// </summary>
        public int MaxPackagesAmount;
    }
}