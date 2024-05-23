using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Level;

namespace DerailedDeliveries.Framework.UI.TextUpdaters
{
    /// <summary>
    /// A <see cref="TextUpdater"/> class that updates the label of a station.
    /// </summary>
    public class StationTextUpdater : TextUpdater
    {
        [SerializeField]
        private TrainStation _parentStation;

        public void SetNewParentStation(TrainStation newParentStation)
        {
            _parentStation = newParentStation;
        }

        private void OnEnable()
        {
            if (_parentStation != null)
                _parentStation.OnStationLabelChange += UpdateStationLabel;
        }

        private void OnDisable()
        {
            if(_parentStation != null)
                _parentStation.OnStationLabelChange -= UpdateStationLabel;
        }

        private void UpdateStationLabel(string newLabel) => ReplaceTag(tag, newLabel);
    }
}