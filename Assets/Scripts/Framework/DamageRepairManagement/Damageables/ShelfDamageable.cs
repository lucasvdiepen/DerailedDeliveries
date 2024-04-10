using FishNet.Object;

namespace DerailedDeliveries.Framework.DamageRepairManagement.Damageables
{
    public class ShelfDamageable : TrainDamageable, IRepairable
    {
        [ServerRpc(RequireOwnership = false)]
        public void Repair() => ChangeHealth(MaxHealth);
    }
}