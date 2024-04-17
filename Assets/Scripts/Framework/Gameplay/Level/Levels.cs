using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Level
{
    /// <summary>
    /// A scriptable object holding a list of all levels.
    /// </summary>
    [CreateAssetMenu(fileName = "Levels", menuName = "ScriptableObjects/Levels")]
    public class Levels : ScriptableObject
    {
        /// <summary>
        /// The list that holds the levels.
        /// </summary>
        public List<LevelData> levels = new();
    }
}