namespace DerailedDeliveries.Framework.DamageRepairManagement
{
    public interface IRepairable
    {
        public abstract void Repair();

        public abstract bool CanBeRepaired();
    }
}