using Cinemachine;
using UnityEngine;
using DG.Tweening;

using DerailedDeliveries.Framework.Train;
using DerailedDeliveries.Framework.StateMachine;
using DerailedDeliveries.Framework.StateMachine.States;

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
        private float _badRailSplitShakeFrequencyPenalty = 0.04f;

        [SerializeField]
        private float _ShakeFrequencyDuration = 1f;

        private CinemachineVirtualCamera _trainCamera;
        private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

        private float _startCameraNoiseAmplitude;
        private float _startCameraNoiseFrequency;

        private void OnEnable() => StateMachine.StateMachine.Instance.OnStateChanged += HandleStateChanged;

        private void HandleStateChanged(State state)
        {
            if (state is not GameState)
                return;

            TrainEngine.Instance.OnSpeedChanged += HandleSpeedChanged;
            _trainController.OnRailSplitChange += HandleRailSplitChanged;
        }

        private void OnDisable()
        {
            if(TrainEngine.Instance != null)
                TrainEngine.Instance.OnSpeedChanged -= HandleSpeedChanged;

            if(_trainController != null)
                _trainController.OnRailSplitChange -= HandleRailSplitChanged;

            if(StateMachine.StateMachine.Instance != null)
                StateMachine.StateMachine.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void Awake()
        {
            _trainCamera = GetComponent<CinemachineVirtualCamera>();
            _multiChannelPerlin = _trainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Start()
        {
            _startCameraNoiseAmplitude = _multiChannelPerlin.m_AmplitudeGain;
            _startCameraNoiseFrequency = _multiChannelPerlin.m_FrequencyGain;

            _multiChannelPerlin.m_AmplitudeGain = 0;
        }

        private void HandleRailSplitChanged(bool badRailSplit)
        {
            float newCameraFrequency = badRailSplit ? _badRailSplitShakeFrequencyPenalty : _startCameraNoiseFrequency;

            DOTween.To(() => _multiChannelPerlin.m_FrequencyGain, x 
                => _multiChannelPerlin.m_FrequencyGain = x, newCameraFrequency, _ShakeFrequencyDuration);
        }

        private void HandleSpeedChanged(float newSpeed)
        {
            // Adjust Cinemachine noise amplitude gain based on current speed
            float amplitudeGain = Mathf.Lerp(0f, _startCameraNoiseAmplitude, Mathf.Abs(newSpeed) / TrainEngine.Instance.MaxSpeed);
            _multiChannelPerlin.m_AmplitudeGain = amplitudeGain;
        }
    }
}
