using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Player;
using DerailedDeliveries.Framework.Train;
using FishNet.Object;
using DerailedDeliveries.Framework.Audio;

namespace DerailedDeliveries.Framework.Gameplay.Interactions.Interactables
{
    /// <summary>
    /// An <see cref="Interactable"/> class that handles logic for the train speed button interactable.
    /// </summary>
    public class TrainSpeedButtonInteractable : Interactable
    {
        [SerializeField]
        private bool _isForwardButton;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="interactor"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool CheckIfUseable(Interactor interactor)
            => IsInteractable && !IsOnCooldown && interactor.InteractingTarget == null;

        private protected override bool Use(Interactor interactor)
        {
            if(!base.Use(interactor))
                return false;

            TrainEngine.Instance.AdjustSpeed(_isForwardButton);
            PlayUseSound();

            return true;
        }

        [ObserversRpc(RunLocally = true, BufferLast = true)]
        private void PlayUseSound() => AudioSystem.Instance.PlayRandomSoundEffectOfType(AudioCollectionTypes.Button, true);
    }
}