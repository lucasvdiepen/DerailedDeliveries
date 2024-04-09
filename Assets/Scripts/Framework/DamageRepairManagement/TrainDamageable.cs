using System.Collections;
using UnityEngine;

namespace DerailedDeliveries.Framework.DamageRepairManagement
{
    public class TrainDamageable : Damageable
    {
        [SerializeField]
        private float _damageInterval;

        private Coroutine _damageIntervalCoroutine;

        private void StartDamageInterval()
        {
            if(_damageIntervalCoroutine != null)
                return;

            _damageIntervalCoroutine = StartCoroutine(DamageIntervalLoop());
        }

        private void StopDamageInterval()
        {
            if(_damageIntervalCoroutine == null)
                return;

            StopCoroutine(_damageIntervalCoroutine);
        }

        private IEnumerator DamageIntervalLoop()
        {
            while(true)
            {
                yield return new WaitForSeconds(_damageInterval);
                TakeDamage();
            }
        }

        private void OnVelocityChanged(float velocity)
        {
            if(Mathf.Abs(velocity) > 0.1f)
                StartDamageInterval();
            else
                StopDamageInterval();
        }

        public override void OnStartServer()
        {
            base.OnStartClient();

            // todo: subscribe to velocity changed event.
        }

        public override void OnStopServer()
        {
            base.OnStopClient();

            // todo: unsubscribe from velocity changed event.
        }
    }
}