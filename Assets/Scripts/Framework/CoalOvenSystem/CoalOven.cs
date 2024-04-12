using FishNet.Object;
using System;
using UnityEngine;

using DerailedDeliveries.Framework.Train;
using DerailedDeliveries.Framework.Utils;

namespace DerailedDeliveries.Framework.CoalOvenSystem
{
    public class CoalOven : NetworkAbstractSingleton<CoalOven>
    {
        public Action<float> OnCoalAmountChanged;

        [SerializeField]
        private float _maxCoalAmount = 100;

        [SerializeField]
        private float _coalBurnRate = 0.833f;

        [SerializeField]
        private float _coalBurnInterval = 1f;

        public bool IsOvenEnabled => TrainEngine.Instance.EngineState == TrainEngineState.Active;

        public float CoalAmount { get; private set; }

        private float _coalToBurn;
        private float _coalBurnIntervalElapsed;

        private void Update()
        {
            if(!IsServer)
                return;

            BurnCoal();
        }

        [Server]
        private void BurnCoal()
        {
            if(!IsOvenEnabled)
                return;

            _coalToBurn += _coalBurnRate * Mathf.Abs(TrainEngine.Instance.CurrentSpeedIndex) * Time.deltaTime;
            _coalBurnIntervalElapsed += Time.deltaTime;

            if(_coalBurnIntervalElapsed >= _coalBurnInterval)
            {
                RemoveCoal(_coalToBurn);
                _coalToBurn = 0;
                _coalBurnIntervalElapsed = 0;
            }
        }

        [Server]
        public void EnableOven()
        {
            if(IsOvenEnabled || CoalAmount < 0.0001f)
                return;

            TrainEngine.Instance.SetEngineState(TrainEngineState.Active);
        }

        [Server]
        public void AddCoal(float amount)
        {
            SetCoalAmount(Mathf.Min(CoalAmount + amount, _maxCoalAmount));
        }

        [Server]
        public void RemoveCoal(float amount)
        {
            SetCoalAmount(Mathf.Max(CoalAmount - amount, 0));
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void SetCoalAmount(float coalAmount)
        {
            CoalAmount = coalAmount;

            if(IsServer)
            {
                if(CoalAmount <= 0.0001f)
                {
                    TrainEngine.Instance.SetEngineState(TrainEngineState.Inactive);
                }
            }

            OnCoalAmountChanged?.Invoke(CoalAmount);
        }
    }
}