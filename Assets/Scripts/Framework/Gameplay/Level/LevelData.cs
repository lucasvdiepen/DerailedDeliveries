using System;

namespace DerailedDeliveries.Framework.Gameplay.Level
{
    /// <summary>
    /// A struct that holds all the level data of a level.
    /// </summary>
    [Serializable]
    public struct LevelData
    {
        /// <summary>
        /// The level data for every station.
        /// </summary>
        public StationLevelData[] StationLevelData;
    }
}