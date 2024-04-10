using FishNet.Object;

namespace DerailedDeliveries.Framework.DamageRepairManagement.Damageables
{
    public class ShelfDamageable : TrainDamageable, IRepairable
    {
        [Server]
        public bool CanBeRepaired() => Health < MaxHealth;

        [Server]
        public void Repair()
        {
            p_damageIntervalElapsed = 0;
            ChangeHealth(MaxHealth);
        }
    }
}