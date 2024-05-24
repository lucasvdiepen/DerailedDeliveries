using UnityEngine;

using DerailedDeliveries.Framework.Train;
using DerailedDeliveries.Framework.Audio;
using DerailedDeliveries.Framework.StateMachine.States;

namespace DerailedDeliveries.Framework.Gameplay.TrainControls
{
    /// <summary>
    /// A class that updates the rotation of a lever based on the current direction of the train.
    /// </summary>
    public class TrainLeverUpdater : MonoBehaviour
    {
        [SerializeField]
        private float _switchUseSoundVolume = .6f;

        [SerializeField]
        private float _leftRotation;

        [SerializeField]
        private float _rightRotation;

        [SerializeField]
        private Transform _leverTransform;

        private void OnEnable()
        {
            TrainEngine.Instance.OnDirectionChanged += UpdateLever;
            UpdateLever(TrainEngine.Instance.CurrentSplitDirection);
        }

        private void OnDisable() => TrainEngine.Instance.OnDirectionChanged -= UpdateLever;

        private void UpdateLever(bool isDirectionRight)
        {
            float newZRotation = isDirectionRight
                ? _rightRotation 
                : _leftRotation;

            if(StateMachine.StateMachine.Instance.CurrentState is GameState)
                AudioSystem.Instance.PlayRandomSoundEffectOfType(AudioCollectionTypes.Switch, true, _switchUseSoundVolume);
                
            _leverTransform.localRotation = Quaternion.Euler(_leverTransform.localRotation.x, _leverTransform.localRotation.y, newZRotation);
        }
    }
}