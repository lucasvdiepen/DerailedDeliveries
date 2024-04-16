using DerailedDeliveries.Framework.PopupManagement;
using DerailedDeliveries.Framework.Train;

namespace DerailedDeliveries.Framework.CoalOvenSystem
{
    public class MatchPopup : Popup
    {
        private void OnEnable()
        {
            TrainEngine.Instance.OnEngineStateChanged += OnEngineStateChanged;
            OnEngineStateChanged(TrainEngine.Instance.EngineState);
        }

        private void OnDisable() => TrainEngine.Instance.OnEngineStateChanged += OnEngineStateChanged;

        private void OnEngineStateChanged(TrainEngineState newTrainEngineState)
        {
            if(newTrainEngineState == TrainEngineState.Inactive)
            {
                Show();
                return;
            }

            Close();
        }
    }
}