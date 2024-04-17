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
        /// The minimum amount of deliverable packages to spawn on this station.
        /// </summary>
        public int minDeliverablePackages;

        /// <summary>
        /// The maximum amount of packages to spawn on this station.
        /// </summary>
        public int maxPackagesAmount;
    }
}