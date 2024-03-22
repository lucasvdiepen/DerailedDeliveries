using UnityEngine.Splines;
using UnityEngine;
using System;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Class responsible for moving train along rails spline.
    /// </summary>
    [RequireComponent(typeof(TrainEngine))]
    public class TrainController : MonoBehaviour
    {
        [SerializeField]
        private SplineContainer _spline;
        
        [Header("Train Config")]
        [SerializeField, Range(0, 1)]
        private float _trainFrontStartTime = 0f;

        [SerializeField]
        private float _heightOffset = 1.0f;

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
        /// Current distance value along spline lenght clamped between 0-1 (same as time). <br/>
        /// <br/> 0 = Spline start point.<br/>
        /// 1 = Spline end point.<br/>
        /// </summary>
        public float DistanceAlongSpline { get; private set; } = 0.0f;

        /// <summary>
        /// Reference to the train engine.
        /// </summary>
        public TrainEngine TrainEngine { get; private set; }

        private void Awake()
        {
            TrainEngine = GetComponent<TrainEngine>();
            DistanceAlongSpline = _trainFrontStartTime;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!gameObject.activeSelf || Application.isPlaying)
                return;

            DebugSnapToSpline();
        }
#endif

        private void Update() 
            => MoveTrain();

        private void MoveTrain()
        {
            UpdateWagonPosition(_frontWagon);

            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
            {
                float adjustedFollowDistance = _wagonFollowDistance / 10f;
                float offset = adjustedFollowDistance + (-_wagonSpacing / 10f) * i;

                UpdateWagonPosition(_wagons[i - 1], offset);
            }

            DistanceAlongSpline += TrainEngine.CurrentVelocity * Time.deltaTime;
            if (DistanceAlongSpline > 1.0f)
                DistanceAlongSpline = 0.0f;
        }

        /// <summary>
        /// Sets a wagon transform to the correct spline position and rotation.
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

#if UNITY_EDITOR
        private void DebugSnapToSpline()
        {
            DistanceAlongSpline = _trainFrontStartTime;
            UpdateWagonPosition(_frontWagon);

            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
            {
                float adjustedFollowDistance = _wagonFollowDistance / 10f;
                float offset = adjustedFollowDistance + (-_wagonSpacing / 10f) * i;
                UpdateWagonPosition(_wagons[i - 1], offset);
            }
        }
#endif
    }
}
