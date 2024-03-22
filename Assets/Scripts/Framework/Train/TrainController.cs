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
        /// <summary>
        /// Reference to the spline line data.
        /// </summary>
        [field: SerializeField]
        public SplineContainer Spline { get; private set; }
        
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

        private RailSplit _railSplit;

        /// <summary>
        /// Current distance value along spline lenght clamped between 0-1 (same as time). <br/>
        /// <br/> 0 = Spline start point.<br/>
        /// 1 = Spline end point.<br/>
        /// </summary>
        public float DistanceAlongSpline { get; private set; } = 0.0f;

        /// <summary>
        /// Returns the precalculated line lenght of the spline
        /// </summary>
        public float SplineLenght { get; private set; } = 0;

        /// <summary>
        /// Reference to the train engine.
        /// </summary>
        public TrainEngine TrainEngine { get; private set; }

        /// <summary>
        /// Helper method for updating the current spline lenght.
        /// </summary>
        public void RecalculateSplineLenght() => SplineLenght = Spline.CalculateLength();

        private void Awake()
        {
            TrainEngine = GetComponent<TrainEngine>();
            DistanceAlongSpline = _trainFrontStartTime;

            if (Spline != null)
                SplineLenght = Spline.CalculateLength();

            _railSplit = Spline.gameObject.GetComponent<RailSplit>();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!gameObject.activeSelf)
                return;

            if (SplineLenght == 0)
                SplineLenght = Spline.CalculateLength();

            if (Application.isPlaying)
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

                UpdateWagonPosition(_wagons[i - 1], offset / SplineLenght);
            }

            DistanceAlongSpline += TrainEngine.CurrentVelocity * Time.deltaTime;
            if (DistanceAlongSpline > 1.0f)
            {
                if(_railSplit == null)
                {
                    print("End reached");
                    return;
                }

                DistanceAlongSpline = 0.0f;
                Spline = _railSplit.GetRandomWay();

                RecalculateSplineLenght();
                Spline.gameObject.TryGetComponent(out _railSplit);
            }
        }

        /// <summary>
        /// Sets a wagon transform to the correct spline position and rotation.
        /// </summary>
        /// <param name="trainBody"></param>
        /// <param name="offset"></param>
        public void UpdateWagonPosition(Transform trainBody, float offset = 0)
        {
            Vector3 nextPosition = Spline.EvaluatePosition(DistanceAlongSpline + (offset / 10f));
            nextPosition.y += _heightOffset;
            trainBody.position = nextPosition;

            Vector3 nextDirection = Spline.EvaluateTangent(DistanceAlongSpline + (offset / 10f));
            trainBody.rotation = Quaternion.LookRotation(-nextDirection, Vector3.up);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Helper method to snap train wagons to the correct spline position/rotation. 
        /// </summary>
        public void DebugSnapToSpline()
        {
            DistanceAlongSpline = _trainFrontStartTime;
            UpdateWagonPosition(_frontWagon);

            int wagons = _wagons.Length + 1;
            for (int i = 1; i < wagons; i++)
            {
                float adjustedFollowDistance = _wagonFollowDistance / 10f;
                float offset = adjustedFollowDistance + (-_wagonSpacing / 10f) * i;
                
                UpdateWagonPosition(_wagons[i - 1], offset / SplineLenght);
            }
        }
#endif
    }
}
