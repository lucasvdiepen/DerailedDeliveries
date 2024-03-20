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

        [SerializeField]
        private float _startDistance = 0f;

        private float _distanceAlongSpline = 0.0f;

        private void Start()
        {
            _distanceAlongSpline = _startDistance;
        }

        [ContextMenu("Snap Train To Spline")]
        public void DebugSnapToSpline()
        {
            _distanceAlongSpline = _startDistance;

            Vector3 nextPosition = _spline.EvaluatePosition(_distanceAlongSpline);
            nextPosition.y += _heightOffset;
            transform.position = nextPosition;

            Vector3 nextDirection = _spline.EvaluateTangent(_distanceAlongSpline);
            transform.rotation = Quaternion.LookRotation(-nextDirection, Vector3.up);
        }

        private void Update()
        {
            Vector3 nextPosition = _spline.EvaluatePosition(_distanceAlongSpline);

            nextPosition.y += _heightOffset;
            transform.position = nextPosition;

            Vector3 nextDirection = _spline.EvaluateTangent(_distanceAlongSpline);
            transform.rotation = Quaternion.LookRotation(-nextDirection, Vector3.up);

            _distanceAlongSpline += _speed * Time.deltaTime;
            if (_distanceAlongSpline > 1.0f)
                _distanceAlongSpline = 0.0f;
        }
    }
}
