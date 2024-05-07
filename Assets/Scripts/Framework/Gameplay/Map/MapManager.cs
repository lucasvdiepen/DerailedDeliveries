using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

using DerailedDeliveries.Framework.Train;
using DG.Tweening;
using System.Collections;
using System;
using Unity.VisualScripting;

namespace DerailedDeliveries.Framework.Gameplay.Map
{
    /// <summary>
    /// A class that is responsible for updating the visuals of the map.
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [SerializeField]
        private MapTrack[] _tracks;

        [SerializeField]
        private MapTrack _currentTrack;

        [SerializeField]
        private TrainController _train;

        [SerializeField]
        private RectTransform _mapIndicator;

        private void Awake()
        {
            UpdateTrackID(0);
            _train.OnTrackSwitch += UpdateTrackID;

            _train.OnDistanceAlongSplineChanged += UpdateDistanceAlongSpline;
        }

        private void UpdateTrackID(int newTrackID)
        {
            for(int i = 0; i < _tracks.Length; i++)
                if (_tracks[i].TrackID == newTrackID)
                    _currentTrack = _tracks[i];
        }

        private void UpdateDistanceAlongSpline(float distanceAlongSpline)
        {
            if (_currentTrack == null)
                return;

            float pathDivider = 1f / _currentTrack.MapPath.Length;
            int index = Mathf.FloorToInt(distanceAlongSpline / pathDivider);

            if (index + 1 >= _currentTrack.MapPath.Length)
                return;

            float lerpAlpha = distanceAlongSpline % pathDivider;

            _mapIndicator.position = Vector3.Lerp
                (
                    _currentTrack.MapPath[index].position, 
                    _currentTrack.MapPath[index + 1].position, 
                    lerpAlpha
                );
        }
    }
}