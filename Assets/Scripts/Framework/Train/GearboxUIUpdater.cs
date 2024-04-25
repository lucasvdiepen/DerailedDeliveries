using UnityEngine;

namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// A class responsible for updating the gearbox UI.
    /// </summary>
    public class GearboxUIUpdater : MonoBehaviour
    {
        [SerializeField]
        private Transform _handle;

        [SerializeField]
        private float[] _handlePositions;

        private void OnEnable()
        {
            TrainEngine.Instance.OnSpeedStateChanged += HandleSpeedStateChanged;
            HandleSpeedStateChanged(TrainEngine.Instance.CurrentGearIndex);
        }

        private void OnDisable()
        {
            if(TrainEngine.Instance == null)
                return;

            TrainEngine.Instance.OnSpeedStateChanged -= HandleSpeedStateChanged;
        }

        private void HandleSpeedStateChanged(int newSpeedState)
        {
            float newHandleRotation = _handlePositions[newSpeedState + TrainEngine.SPEED_VALUES_COUNT];
            _handle.rotation = Quaternion.Euler(0, 0, newHandleRotation);
        }
    }
}