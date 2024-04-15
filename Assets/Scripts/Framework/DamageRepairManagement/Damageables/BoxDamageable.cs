using FishNet.Object;
using UnityEngine;

namespace DerailedDeliveries.Framework.DamageRepairManagement.Damageables
{
    /// <summary>
    /// A <see cref="TrainDamageable"/> class which handles logic for the box.
    /// </summary>
    public class BoxDamageable : TrainDamageable
    {
        [SerializeField]
        private LayerMask _trainLayer;

        private int _amountInTrain;

        [Server]
        private protected override void UpdateTimer()
        {
            if(_amountInTrain == 0)
                return;

            base.UpdateTimer();
        }

        [Server]
        private protected override void TakeDamage()
        {
            if(_amountInTrain == 0)
                return;

            base.TakeDamage();
        }

        private void OnTriggerEnter(Collider other)
        {
            if((_trainLayer.value & 1 << other.gameObject.layer) == 0)
                return;

            _amountInTrain++;
        }

        private void OnTriggerExit(Collider other)
        {
            if((_trainLayer.value & 1 << other.gameObject.layer) == 0)
                return;

            _amountInTrain--;
        }
    }
}