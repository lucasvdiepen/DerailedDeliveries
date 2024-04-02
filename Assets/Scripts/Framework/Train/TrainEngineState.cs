namespace DerailedDeliveries.Framework.Train
{
    public enum TrainEngineState
    {
        /// <summary>
        /// State when train engine is not running.
        /// </summary>
        off,

        /// <summary>
        /// State when train engine is running and active.
        /// </summary>
        on,
        
        /// <summary>
        /// State when train engine is running and inactive.
        /// </summary>
        onStandby,
    }
}
