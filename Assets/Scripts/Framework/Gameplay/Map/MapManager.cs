using UnityEngine;

using DerailedDeliveries.Framework.Train;

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
        private RectTransform _mapIndicator;

        private void Awake()
        {
            UpdateTrackID(0);
            _trainController.OnTrackSwitch += UpdateTrackID;

            _trainController.OnDistanceAlongSplineChanged += UpdateDistanceAlongSpline;
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

            Vector3 currentPos = _currentTrack.MapPath[index].position;
            Vector3 endPos = _currentTrack.MapPath[index + 1].position;

            _mapIndicator.position = Vector3.Lerp
                (
                    currentPos,
                    endPos,
                    lerpAlpha
                );
        }
    }
}