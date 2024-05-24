using FishNet.Object;
using System;
using UnityEngine;

using DerailedDeliveries.Framework.Train;
using DerailedDeliveries.Framework.Utils;
using DerailedDeliveries.Framework.DamageRepairManagement;
using DerailedDeliveries.Framework.Audio;

namespace DerailedDeliveries.Framework.CoalOvenSystem
{
    /// <summary>
    /// A class responsible for controlling the coal oven.
    /// </summary>
    [RequireComponent(typeof(TrainDamageable))]
    public class CoalOven : NetworkAbstractSingleton<CoalOven>
    {
        [SerializeField]
        private float _maxCoalAmount = 100;

        [SerializeField]
        private float _coalBurnRate = 0.25f;

        [SerializeField]
        private float _coalBurnInterval = 1f;

        [SerializeField]
        private bool _ignoreCoalBurn;

        /// <summary>
        /// Invoked when the coal amount changes.
        /// </summary>
        public Action<float> OnCoalAmountChanged;

        /// <summary>
        /// Gets whether the oven is enabled or not.
        /// </summary>
        public bool IsOvenEnabled {  get; private set; }    

        /// <summary>
        /// Gets the current coal amount.
        /// </summary>
        public float CoalAmount { get; private set; }

        /// <summary>
        /// Gets the maximum coal amount.
        /// </summary>
        public float MaxCoalAmount => _maxCoalAmount;

        private float _coalToBurn;
        private float _coalBurnIntervalElapsed;
        private TrainDamageable _damageable;

        private void Awake() => _damageable = GetComponent<TrainDamageable>();

        private void OnEnable()
        {
            _damageable.OnHealthChanged += OnHealthChanged;
            OnHealthChanged(_damageable.Health);

            TrainEngine.Instance.OnEngineStateChanged += HandleEngineStateChanged;
        }

        private void HandleEngineStateChanged(TrainEngineState state)
        {
            IsOvenEnabled = state == TrainEngineState.Active;

            if (!IsOvenEnabled && IsServer)
                PlayOvenSound(false);
        }

        private void OnDisable()
        {
            _damageable.OnHealthChanged -= OnHealthChanged;

            if(TrainEngine.Instance != null)
                TrainEngine.Instance.OnEngineStateChanged -= HandleEngineStateChanged;
        }

        private void Update()
        {
            if(!IsServer)
                return;

            BurnCoal();
        }

        /// <summary>
        /// Enables the oven.
        /// </summary>
        [Server]
        public void EnableOven()
        {
            if((IsOvenEnabled || CoalAmount < 0.0001f || _damageable.Health <= 0) && !_ignoreCoalBurn)
                return;

            TrainEngine.Instance.SetEngineState(TrainEngineState.Active);
            PlayOvenSound(true);
        }

        /// <summary>
        /// Adds coal to the oven.
        /// </summary>
        /// <param name="amount">The amount of coal to add.</param>
        [Server]
        public void AddCoal(float amount) => SetCoalAmount(Mathf.Min(CoalAmount + amount, _maxCoalAmount));

        /// <summary>
        /// Removes coal from the oven.
        /// </summary>
        /// <param name="amount">The amount of coal to remove.</param>
        [Server]
        public void RemoveCoal(float amount) => SetCoalAmount(Mathf.Max(CoalAmount - amount, 0));

        [Server]
        private void BurnCoal()
        {
            if(!IsOvenEnabled || _ignoreCoalBurn)
                return;

            _coalToBurn += _coalBurnRate * Mathf.Abs(TrainEngine.Instance.CurrentGearIndex) * Time.deltaTime;
            _coalBurnIntervalElapsed += Time.deltaTime;

            if(_coalBurnIntervalElapsed >= _coalBurnInterval)
            {
                RemoveCoal(_coalToBurn);
                _coalToBurn = 0;
                _coalBurnIntervalElapsed = 0;
            }
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void SetCoalAmount(float coalAmount)
        {
            CoalAmount = coalAmount;

            if(IsServer && CoalAmount <= 0.0001f)
                TrainEngine.Instance.SetEngineState(TrainEngineState.Inactive);

            OnCoalAmountChanged?.Invoke(CoalAmount);
        }

        private void OnHealthChanged(int health)
        {
            if(health > 0)
                return;

            TrainEngine.Instance.SetEngineState(TrainEngineState.Inactive);
        }

        [ObserversRpc(RunLocally = true)]
        private void PlayOvenSound(bool isOvenEnabled)
            => AudioSystem.Instance.PlayRandomSoundEffectOfType(isOvenEnabled 
                ? AudioCollectionTypes.CoalOvenOn 
                : AudioCollectionTypes.CoalOvenOff);
    }
}