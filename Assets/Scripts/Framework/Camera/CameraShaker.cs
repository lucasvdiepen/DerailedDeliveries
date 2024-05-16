using Cinemachine;
using DerailedDeliveries.Framework.Train;
using UnityEngine;

namespace DerailedDeliveries.Framework.Camera
{
    /// <summary>
    /// Class responsible for shaking the camera depending on the speed of the train.
    /// </summary>
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraShaker : MonoBehaviour
    {
        private CinemachineVirtualCamera _trainCamera;
        private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

        private float _startCameraNoiseAmplitude;

        private void OnEnable() => TrainEngine.Instance.OnSpeedChanged += HandleSpeedChanged;

        private void OnDisable()
        {
            if(TrainEngine.Instance != null)
                TrainEngine.Instance.OnSpeedChanged -= HandleSpeedChanged;
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

        private void HandleSpeedChanged(float newSpeed)
        {
            // Adjust Cinemachine noise amplitude gain based on current speed
            float amplitudeGain = Mathf.Lerp(0f, _startCameraNoiseAmplitude, Mathf.Abs(newSpeed) / TrainEngine.Instance.MaxSpeed);
            _multiChannelPerlin.m_AmplitudeGain = amplitudeGain;
        }
    }
}
