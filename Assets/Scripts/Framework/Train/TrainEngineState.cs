namespace DerailedDeliveries.Framework.Train
{
    public enum TrainEngineState
    {
        /// <summary>
        /// State when train engine is not running.
        /// </summary>
        OFF,

        /// <summary>
        /// State when train engine is running and active.
        /// </summary>
        ON,
        
        /// <summary>
        /// State when train engine is running and inactive.
        /// </summary>
        ON_STANDBY
    }
}
