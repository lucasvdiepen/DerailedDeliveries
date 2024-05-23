using UnityEngine;
using TMPro;

using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.UI.DoorStateUpdater
{
    /// <summary>
    /// Class responsible for updating the UI door state indicator
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DoorStateTextUpdater : MonoBehaviour
    {
        private TextMeshProUGUI _doorStateLabel;

        private void Awake() => _doorStateLabel = GetComponent<TextMeshProUGUI>();

        private void OnEnable() => TrainStationController.Instance.OnParkStateChanged += HandleParkStateChanged;

        private void OnDisable()
        {
            if(TrainStationController.Instance != null)
                TrainStationController.Instance.OnParkStateChanged -= HandleParkStateChanged;
        }

        private void Start() => HandleParkStateChanged(true);

        private void HandleParkStateChanged(bool newParkState) 
            => _doorStateLabel.SetText($"Door state: {(newParkState ? "OPENED" : "CLOSED")}");
    }
}
