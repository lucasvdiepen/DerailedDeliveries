using FishNet.Object;

namespace DerailedDeliveries.Framework.DamageRepairManagement.Damageables
{
    /// <summary>
    /// A <see cref="TrainDamageable"/> class which handles logic for the shelf.
    /// </summary>
    public class ShelfDamageable : TrainDamageable, IRepairable
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        [Server]
        public bool CanBeRepaired() => Health < MaxHealth;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        [Server]
        public void Repair()
        {
            p_damageIntervalElapsed = 0;
            ChangeHealth(MaxHealth);
        }
    }
}