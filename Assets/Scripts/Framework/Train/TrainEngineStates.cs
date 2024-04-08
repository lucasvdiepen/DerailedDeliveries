namespace DerailedDeliveries.Framework.Train
{
    public enum TrainEngineStates
    {
        /// <summary>
        /// State when train engine is not running.
        /// </summary>
        Off,

        /// <summary>
        /// State when train engine is running and active.
        /// </summary>
        Running,
        
        /// <summary>
        /// State when train engine is running and inactive.
        /// </summary>
        OnStandby,
    }
}
