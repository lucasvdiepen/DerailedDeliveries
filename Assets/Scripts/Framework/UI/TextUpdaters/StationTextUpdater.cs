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

        private void OnEnable()
        {
            if (_parentStation != null)
                UpdateStationLabel();
        }

        private void UpdateStationLabel() => ReplaceTag(_parentStation.StationLabel);
    }
}