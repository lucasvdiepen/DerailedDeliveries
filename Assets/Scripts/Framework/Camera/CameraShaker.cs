using Cinemachine;
using UnityEngine;

using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.Camera
{
    /// <summary>
    /// Class responsible for shaking the camera depending on the speed of the train.
    /// </summary>
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField]
        private TrainController _trainController;

        [SerializeField]
        private float _badRailSplitShakePenalty = 10f;

        private bool _applyShakePenalty;

        private CinemachineVirtualCamera _trainCamera;
        private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

        private float _startCameraNoiseAmplitude;

        private void OnEnable()
        {
            TrainEngine.Instance.OnSpeedChanged += HandleSpeedChanged;
            _trainController.onRailSplitChange += HandleRailSplitChanged;
        }

        private void OnDisable()
        {
            if(TrainEngine.Instance != null)
                TrainEngine.Instance.OnSpeedChanged -= HandleSpeedChanged;

            if(_trainController != null)
                _trainController.onRailSplitChange -= HandleRailSplitChanged;
        }

        private void Awake()
        {
            _trainCamera = GetComponent<CinemachineVirtualCamera>();
            _multiChannelPerlin = _trainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Start()
        {
            _startCameraNoiseAmplitude = _multiChannelPerlin.m_AmplitudeGain;
            _multiChannelPerlin.m_AmplitudeGain = 0;
        }

        private void HandleRailSplitChanged(int newRailSplitIndex, RailSplitType nextRailSplitType)  
        {
            if (nextRailSplitType == RailSplitType.Branch)
            {
                bool badSplitDirection = _trainController.BadRailSplitOrder[newRailSplitIndex - 1];
                
                if(badSplitDirection == TrainEngine.Instance.CurrentSplitDirection)
                    _applyShakePenalty = true;
            }
            else
            {
                _applyShakePenalty = false;
            }
        }

        private void HandleSpeedChanged(float newSpeed)
        {
            // Adjust Cinemachine noise amplitude gain based on current speed
            float amplitudeGain = Mathf.Lerp(0f, _startCameraNoiseAmplitude, Mathf.Abs(newSpeed) / TrainEngine.Instance.MaxSpeed);
            _multiChannelPerlin.m_AmplitudeGain = amplitudeGain * (_applyShakePenalty ? _badRailSplitShakePenalty : 0);
        }
    }
}
