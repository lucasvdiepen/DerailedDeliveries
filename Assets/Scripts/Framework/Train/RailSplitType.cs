namespace DerailedDeliveries.Framework.Train
{
    /// <summary>
    /// Enum responsible for differentiating between rail split types.
    /// </summary>
    public enum RailSplitType
    {
        /// <summary>
        /// Rail split connection type for when a single rail track splits of into two other rail tracks.
        /// </summary>
        Branch,

        /// <summary>
        /// Rail split connection type for when two different rail tracks merge into a single rail track.
        /// </summary>
        Funnel
    }
}