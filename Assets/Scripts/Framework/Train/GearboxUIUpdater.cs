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

        private void OnEnable() =>
            TrainEngine.Instance.OnSpeedStateChanged += HandleSpeedStateChanged;

        private void OnDisable() =>
            TrainEngine.Instance.OnSpeedStateChanged -= HandleSpeedStateChanged;

        private void HandleSpeedStateChanged(int newSpeedState)
        {
            _handle.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}