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
        private MapTrack _currentTrack;

        [SerializeField]
        private List<Tuple<Vector3, Vector3, float>> _currentMapPath = new();

        [SerializeField]
        private MapTrack[] _tracks;

        [SerializeField]
        private RectTransform _locationIndicator;

        [SerializeField]
        private float _totalLength;

        private void Awake()
        {
            for (int i = 0; i + 1 < _currentTrack.MapPath.Length; i++)
                _totalLength += Vector2.Distance(_currentTrack.MapPath[i].position, _currentTrack.MapPath[i + 1].position);

        }

        private void UpdateTrackID(int newTrackID)
        {
            int trackAmount = _tracks.Length;
            for(int i = 0; i < trackAmount; i++)
                if (_tracks[i].TrackID == newTrackID)
                    _currentTrack = _tracks[i];

            _currentMapPath.Clear();
            int mapPathLength = _currentTrack.MapPath.Length;

            for (int i = 0; i + 1 < mapPathLength; i++)
            {
                Vector3 startPos = _currentTrack.MapPath[i].position;
                Vector3 endPos = _currentTrack.MapPath[i + 1].position;

                Tuple<Vector3, Vector3, float> pathData = new(startPos, endPos, Vector3.Distance(startPos, endPos));
                _currentMapPath.Add(pathData);
            }

            _totalLength = 0;
            int currentMapPathCount = _currentMapPath.Count;

            for (int i = 0; i < currentMapPathCount; i++)
                _totalLength += _currentMapPath[i].Item3;
        }

        private int index = 0;

        private IEnumerator StartLerpBetweenTargets(float distanceAlongSpline)
        {
            RectTransform startPos = _currentTrack.MapPath[index];
            RectTransform endPos = _currentTrack.MapPath[index + 1];

            while()
            {
                _locationIndicator.position = Vector3.Lerp()
            }
        }

        private void CompletePathAndRecurse()
        {
            index++;

            if(index <= _currentTrack.MapPath.Length - 1)
                StartLerpBetweenTargets();
        }
    }
}