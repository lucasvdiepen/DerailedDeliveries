using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DerailedDeliveries.Framework.Gameplay.Map
{
    /// <summary>
    /// A class that holds data and functionality for updating the current position on the map.
    /// </summary>
    public class MapTrack : MonoBehaviour
    {
        /// <summary>
        /// The TrackID of this <see cref="MapTrack"/>.
        /// </summary>
        [field: SerializeField]
        public int TrackID = -1;

        /// <summary>
        /// The array of <see cref="RectTransform"/>s that is lerped from <see cref="RectTransform"/> to 
        /// <see cref="RectTransform"/> to represent the current Track.
        /// </summary>
        [field: SerializeField]
        public RectTransform[] MapPath;
    }
}