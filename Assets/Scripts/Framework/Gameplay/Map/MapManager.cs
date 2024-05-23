using UnityEngine;

using DerailedDeliveries.Framework.UI.TextUpdaters;
using DerailedDeliveries.Framework.Gameplay.Level;
using DerailedDeliveries.Framework.Train;
using System.Collections.Generic;
using System.ComponentModel;

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
        private TrainController _trainController;

        [SerializeField]
        private float _distanceAlongSpline;

        [SerializeField]
        private List<float> distancesForStations;

        [SerializeField]
        private List<TrainStation> trainStations;

        [SerializeField]
        private RectTransform _mapIndicator;

        [SerializeField]
        private GameObject _stationIndicatorPrefab;

        private void Awake()
        {
            UpdateTrackID(0);
            _trainController.OnTrackSwitch += UpdateTrackID;

            _trainController.OnDistanceAlongSplineChanged += UpdateDistanceAlongSpline;

            for(int i = 0; i < _tracks.Length; i++)
                _tracks[i]._badSplitWarning.enabled = true; // _trainController.BadRailSplitOrder[i];
        }

        [ContextMenu("PlaceStationIndicator")]
        private void SpawnStationIndicator()
        {
            GameObject newStationIndicator = Instantiate(_stationIndicatorPrefab);

            newStationIndicator.transform.parent = gameObject.transform;

            newStationIndicator.transform.localRotation = Quaternion.identity;
            newStationIndicator.transform.position = _mapIndicator.position;

            distancesForStations.Add(_distanceAlongSpline);

            TrainStation[] allStations = FindObjectsOfType<TrainStation>();
            TrainStation closestStation = null;

            for(int i = 0; i < allStations.Length; i++)
            {
                if(closestStation == null)
                {
                    closestStation = allStations[i];
                    continue;
                }

                float distanceToOther = Vector3.Distance(transform.position, allStations[i].transform.position);
                float distanceToClosest = Vector3.Distance(transform.position, closestStation.transform.position);

                if(distanceToOther < distanceToClosest)
                    closestStation = allStations[i];
            }

            trainStations.Add(closestStation);
        }

        private void UpdateTrackID(int newTrackID)
        {
            for(int i = 0; i < _tracks.Length; i++)
                if (_tracks[i].TrackID == newTrackID)
                    _currentTrack = _tracks[i];
        }

        private void UpdateDistanceAlongSpline(float distanceAlongSpline)
        {
            _distanceAlongSpline = distanceAlongSpline;

            if (_currentTrack == null)
                return;

            float pathDivider = 1f / _currentTrack.MapPath.Length;
            int index = Mathf.FloorToInt(distanceAlongSpline / pathDivider);

            if (index + 1 >= _currentTrack.MapPath.Length)
                return;

            float lerpAlpha = distanceAlongSpline % pathDivider;

            _mapIndicator.position = ReturnLerpPosition(index, lerpAlpha);
        }

        private Vector3 ReturnLerpPosition(int index, float lerpAlpha)
        {
            Vector3 currentPos = _currentTrack.MapPath[index].position;
            Vector3 endPos = _currentTrack.MapPath[index + 1].position;

            return Vector3.Lerp
                (
                    currentPos,
                    endPos,
                    lerpAlpha
                );
        }
    }
}