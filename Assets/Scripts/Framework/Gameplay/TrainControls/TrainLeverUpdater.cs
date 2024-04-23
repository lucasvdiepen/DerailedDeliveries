using UnityEngine;

using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.Gameplay.TrainControls
{
    public class TrainLeverUpdater : MonoBehaviour
    {
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

            _leverTransform.rotation = Quaternion.Euler(_leverTransform.rotation.x, _leverTransform.rotation.y, newZRotation);
        }
    }
}