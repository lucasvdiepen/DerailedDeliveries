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
        [field: SerializeField]
        public int TrackID = -1;

        [field: SerializeField]
        public RectTransform[] MapPath;
    }
}