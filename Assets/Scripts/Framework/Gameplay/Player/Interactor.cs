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
        [SerializeField]
        private Interactable _interactingTarget;

        [SerializeField]
        private Transform _grabbingAnchor;

        [SerializeField]
        private float _cooldown = .2f;

        private PlayerInputParser _inputParser;

        [SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private bool _isInteracting;

        private bool _isOnCooldown;

        private void Awake() => _inputParser = gameObject.GetComponent<PlayerInputParser>();

        private void OnEnable() => _inputParser.OnInteract += UseInteractable;

        private void OnDisable() => _inputParser.OnInteract -= UseInteractable;

        private void UseInteractable()
        {
            Interactable[] interactables = ComponentsInCollider;

            if (_isOnCooldown || !_isInteracting && interactables.Length == 0)
                return;

            if (_isInteracting)
            {
                StartCoroutine(ActivateCooldown());
                _interactingTarget.InteractOnServer(this);
                return;
            }

            StartCoroutine(ActivateCooldown());

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
        /// A function that sets the InteractingTarget of this Interactor serversided.
        /// </summary>
        /// <param name="interactable">The current interacting target. If the interactor already had this
        /// reference set it will reset it.</param>
        public void SetInteractingTarget(Interactable interactable, bool isInteracting)
        {
            _interactingTarget = interactable;
            _isInteracting = isInteracting;

            if (_isInteracting)
            {
                interactable.NetworkObject.SetParent(_grabbingAnchor.GetComponent<NetworkBehaviour>());
                interactable.gameObject.transform.localPosition = Vector3.zero;
            }
            else
                interactable.NetworkObject.UnsetParent();
        }

        private protected virtual IEnumerator ActivateCooldown()
        {
            _isOnCooldown = true;
            yield return new WaitForSeconds(_cooldown);
            _isOnCooldown = false;
        }
    }
}