namespace DerailedDeliveries.Framework.DamageRepairManagement
{
    /// <summary>
    /// A interface responsible for handling repairable objects.
    /// </summary>
    public interface IRepairable
    {
        /// <summary>
        /// Repairs the object.
        /// </summary>
        public abstract void Repair();

        /// <summary>
        /// Checks if the object can be repaired.
        /// </summary>
        /// <returns>True if the object can be repaired, otherwise false.</returns>
        public abstract bool CanBeRepaired();
    }
}