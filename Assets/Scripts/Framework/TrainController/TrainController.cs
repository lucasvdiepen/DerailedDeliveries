using UnityEngine.Splines;
using UnityEngine;

namespace DerailedDeliveries.Framework.TrainController
{
    /// <summary>
    /// Class responsible for moving train along rails spline.
    /// </summary>
    public class TrainController : MonoBehaviour
    {
        [SerializeField]
        private SplineContainer _spline;

        [SerializeField]
        private float _speed = 1.0f;

        [SerializeField]
        private float _heightOffset = 1.0f;

        [SerializeField, Range(0, 1)]
        private float _trainFrontStartTime = 0f;

        [SerializeField]
        private float _wagonFollowDistance = 0f;

        [SerializeField]
        private float _wagonSpacing = 0f;

        [SerializeField]
        private Transform _frontWagon = null;

        [SerializeField]
        private Transform[] _wagons = null;

        public float CurrentVelocity => _speed / 100f;

        private float _distanceAlongSpline = 0.0f;

        private void Start()
        {
            _distanceAlongSpline = _trainFrontStartTime;
        }

        private void OnValidate()
        {
            if (!gameObject.activeSelf || Application.isPlaying)
                return;

            DebugSnapToSpline();
        }

        /// <summary>
        /// Sets a wagon transform to the correct spline position.
        /// </summary>
        /// <param name="trainBody"></param>
        /// <param name="offset"></param>
        public void UpdateWagonPosition(Transform trainBody, float offset = 0)
        {
            Vector3 nextPosition = _spline.EvaluatePosition(_distanceAlongSpline + (offset / 10f));
            nextPosition.y += _heightOffset;
            trainBody.position = nextPosition;

            Vector3 nextDirection = _spline.EvaluateTangent(_distanceAlongSpline + (offset / 10f));
            trainBody.rotation = Quaternion.LookRotation(-nextDirection, Vector3.up);
        }

        private void Update()
        {
            UpdateWagonPosition(_frontWagon);

            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
            {
                float adjustedFollowDistance = _wagonFollowDistance / 10f;
                float offset = adjustedFollowDistance + (_wagonSpacing / 10f) * i;

                UpdateWagonPosition(_wagons[i - 1], offset);
            }

            _distanceAlongSpline += CurrentVelocity * Time.deltaTime;
            if (_distanceAlongSpline > 1.0f)
                _distanceAlongSpline = 0.0f;
        }

        private void DebugSnapToSpline()
        {
            _distanceAlongSpline = _trainFrontStartTime;
            UpdateWagonPosition(_frontWagon);

            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
            {
                float adjustedFollowDistance = _wagonFollowDistance / 10f;
                float offset = adjustedFollowDistance + (_wagonSpacing / 10f) * i;
                UpdateWagonPosition(_wagons[i - 1], offset);
            }
        }
    }
}
