using FishNet.Object.Synchronizing;
using System.Collections;
using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Interactions;
using DerailedDeliveries.Framework.InputParser;
using DerailedDeliveries.Framework.TriggerArea;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    /// <summary>
    /// A class that is responsible for handling with in range Interactables for the player.
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class Interactor : NetworkTriggerArea<Interactable>
    {
        /// <summary>
        /// Returns the GrabbingAnchor Transform of this Interactor.
        /// </summary>
        public NetworkBehaviour GrabbingAnchor => _grabbingAnchor;

        /// <summary>
        /// A getter that returns the Interactor's InteractingTarget.
        /// </summary>
        public Interactable InteractingTarget => _interactingTarget;

        [SerializeField]
        private Interactable _interactingTarget;

        [SerializeField]
        private NetworkBehaviour _grabbingAnchor;

        [SerializeField]
        private float _cooldown = .2f;

        [SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private bool _isInteracting;
        private PlayerInputParser _inputParser;
        private bool _isOnCooldown;

        private void Awake() => _inputParser = gameObject.GetComponent<PlayerInputParser>();

        private void OnEnable() => _inputParser.OnInteract += UseInteractable;

        private void OnDisable() => _inputParser.OnInteract -= UseInteractable;

        private void UseInteractable()
        {
            Interactable[] interactables = ComponentsInCollider;

            if (_isOnCooldown || !_isInteracting && interactables.Length == 0)
                return;

            StartCoroutine(ActivateCooldown());

            if (_isInteracting)
            {
                _interactingTarget.InteractOnServer(this);
                return;
            }

            _interactingTarget = null;

            foreach(Interactable interactable in interactables)
            {
                if (interactable.CheckIfInteractable())
                {
                    _interactingTarget = interactable;
                    break;
                }
            }

            if(_interactingTarget != null)
                _interactingTarget.InteractOnServer(this);
        }

        /// <summary>
        /// A function that sets the InteractingTarget of this Interactor server sided.
        /// </summary>
        /// <param name="interactable">The current interacting target. If the interactor already had this
        /// reference set it will reset it.</param>
        [Server]
        public void UpdateInteractingTarget(Interactable interactable, bool isInteracting)
        {
            _interactingTarget = interactable;
            _isInteracting = isInteracting;
        }

        private protected virtual IEnumerator ActivateCooldown()
        {
            _isOnCooldown = true;
            yield return new WaitForSeconds(_cooldown);
            _isOnCooldown = false;
        }
    }
}