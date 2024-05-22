using UnityEngine;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// A class responsible for enabling/disabling the steam effect of the train.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class TrainSteamHandler : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Awake() => _particleSystem = GetComponent<ParticleSystem>();

        private void OnEnable() => TrainEngine.Instance.OnEngineStateChanged += OnEngineStateChanged;

        private void OnDisable() => TrainEngine.Instance.OnEngineStateChanged -= OnEngineStateChanged;

        private void OnEngineStateChanged(TrainEngineState trainEngineState)
        {
            if(trainEngineState == TrainEngineState.Active)
            {
                _particleSystem.Play();
                return;
            }

            _particleSystem.Stop();
        }
    }
}