using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.Gameplay.Map
{
    /// <summary>
    /// A class that is responsible for updating the visuals of the map.
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [SerializeField]
        private MapTrack _currentTrack;

        [SerializeField]
        private MapTrack[] _tracks;

        private void Awake()
        {
            
        }

        private void UpdateTrackID(int newTrackID)
        {
            int tracksAmount = _tracks.Length;
            for(int i = 0; i < tracksAmount; i++)
                if (_tracks[i].TrackID == newTrackID)
                    _currentTrack = _tracks[i];
        }
    }
}