namespace DerailedDeliveries.Framework.TrainController
{
    public enum TrainEngineState
    {
        /// <summary>
        /// State when train engine is not running.
        /// </summary>
        OFF,

        /// <summary>
        /// State when train engine is running, but not moving.
        /// </summary>
        ON_STANDBY,

        /// <summary>
        /// State when train engine is running and accelerating speed.
        /// </summary>
        ACCELERATING,

        /// <summary>
        /// State when train engine is running and deaccelerating speed.
        /// </summary>
        DECELERATING,

        /// <summary>
        /// State when train engine is running and has reached peak speed.
        /// </summary>
        FULL_POWER,
    }
}
