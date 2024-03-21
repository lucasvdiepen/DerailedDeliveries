using UnityEngine.Splines;
using UnityEngine;
using DG.Tweening;

namespace DerailedDeliveries.Framework.TrainController
{
    /// <summary>
    /// Class responsible for moving train along rails spline.
    /// </summary>
    public class TrainController : MonoBehaviour
    {
        [SerializeField]
        private SplineContainer _spline;
        
        [SerializeField, Range(0, 1)]
        private float _trainFrontStartTime = 0f;

        [Header("Train Config")]
        [SerializeField]
        private float _maxSpeed = 7.5f;

        [SerializeField]
        private float _heightOffset = 1.0f;
        
        [SerializeField]
        private float _accelerationDuration = 10f;

        [SerializeField]
        private Ease _accelerationEase = Ease.Linear;

        [Header("Wagons Config")]
        [SerializeField]
        private Transform _frontWagon = null;
        
        [SerializeField]
        private float _wagonFollowDistance = 0f;

        [SerializeField]
        private float _wagonSpacing = 0f;

        [SerializeField]
        private Transform[] _wagons = null;

        /// <summary>
        /// Train velocity speed.
        /// </summary>
        public float CurrentVelocity => _currentSpeed / 100f;

        /// <summary>
        /// Current train engine state.
        /// </summary>
        public TrainEngineState EngineState { get; set; }

        /// <summary>
        /// Distance value along spline lenght (same as time).
        /// </summary>
        public float DistanceAlongSpline { get; private set; } = 0.0f;
        
        private float _currentSpeed = 0f;

        private void Start()
        {
            DistanceAlongSpline = _trainFrontStartTime;
            EngineState = TrainEngineState.ON_STANDBY;

            var accelerationTween = DOTween.To(() 
                => _currentSpeed, x => _currentSpeed = x, _maxSpeed, _accelerationDuration);

            accelerationTween.OnStart(() => EngineState = TrainEngineState.ACCELERATING);
            accelerationTween.OnComplete(() => EngineState = TrainEngineState.FULL_POWER);

            DOTween.Sequence()
                .AppendInterval(2f)
                .Append(accelerationTween.SetEase(_accelerationEase));
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
            Vector3 nextPosition = _spline.EvaluatePosition(DistanceAlongSpline + (offset / 10f));
            nextPosition.y += _heightOffset;
            trainBody.position = nextPosition;

            Vector3 nextDirection = _spline.EvaluateTangent(DistanceAlongSpline + (offset / 10f));
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

            DistanceAlongSpline += CurrentVelocity * Time.deltaTime;
            if (DistanceAlongSpline > 1.0f)
                DistanceAlongSpline = 0.0f;
        }

        private void DebugSnapToSpline()
        {
            DistanceAlongSpline = _trainFrontStartTime;
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
