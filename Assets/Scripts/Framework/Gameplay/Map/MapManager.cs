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
        private float _distanceAlongSpline;

        [SerializeField]
        private RectTransform _mapIndicator;

        [SerializeField]
        private GameObject _stationIndicatorPrefab;

        private void Awake()
        {
            UpdateTrackID(0, false);
            _trainController.OnRailSplitChange += UpdateTrackID;
        }

        private void OnEnable()
        {
            UpdateDistanceAlongSpline(_distanceAlongSpline);

            _trainController.OnDistanceAlongSplineChanged += UpdateDistanceAlongSpline;

            int trackIndex = 0;
            for(int i = 0; i < _trainController.BadRailSplitOrder.Length; i++)
            {
                _tracks[trackIndex]._badSplitWarning.enabled = false;
                trackIndex++;

                _tracks[trackIndex]._badSplitWarning.enabled = _trainController.BadRailSplitOrder[i];
                trackIndex++;

                _tracks[trackIndex]._badSplitWarning.enabled = !_trainController.BadRailSplitOrder[i];
                trackIndex++;
            }
        }

        private void OnDisable() => _trainController.OnDistanceAlongSplineChanged -= UpdateDistanceAlongSpline;

        private void UpdateTrackID(int newTrackID, bool isBadRailsplit = false)
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

            float pathDivider = 1f / (_currentTrack.MapPath.Length - 1);
            int index = (int)(distanceAlongSpline / pathDivider);

            if (index + 1 >= _currentTrack.MapPath.Length)
                return;

            float lerpAlpha = (distanceAlongSpline % pathDivider) / pathDivider;

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