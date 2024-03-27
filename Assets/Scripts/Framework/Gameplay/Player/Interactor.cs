using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using System.Collections;
using FishNet.Object;
using UnityEngine;

using DerailedDeliveries.Framework.Gameplay.Interactions;
using DerailedDeliveries.Framework.InputParser;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    /// <summary>
    /// A class that is responsible for handling with in range Interactables for the player.
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class Interactor : NetworkBehaviour
    {
        /// <summary>
        /// Returns the GrabbingAnchor Transform of this Interactor.
        /// </summary>
        public Transform GrabbingAnchor => _grabbingAnchor;

        [SerializeField]
        private List<Interactable> _interactables;

        [SerializeField]
        private Interactable _interactingTarget;

        [SerializeField]
        private Transform _grabbingAnchor;

        [SerializeField]
        private float _cooldown = .2f;


        [SyncVar(Channel = FishNet.Transporting.Channel.Reliable)]
        private bool _isInteracting;

        private PlayerInputParser _inputParser;
        private bool _isOnCooldown;


        private void Awake() => _inputParser = gameObject.GetComponent<PlayerInputParser>();

        private void OnEnable() => _inputParser.OnInteract += UseInteractable;

        private void OnDisable() => _inputParser.OnInteract -= UseInteractable;

        private void OnTriggerEnter(Collider collider)
        {
            GameObject target = collider.gameObject;

            if (target.TryGetComponent(out Interactable interactable))
            {
                if (_interactables.Contains(interactable))
                    return;

                _interactables.Add(interactable);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            GameObject target = collider.gameObject;

            if(target.TryGetComponent(out Interactable interactable))
            {
                if (!_interactables.Contains(interactable))
                    return;

                _interactables.Remove(interactable);
            }
        }

        private void UseInteractable()
        {
            if (_isOnCooldown || !_isInteracting && _interactables.Count == 0)
                return;

            StartCoroutine(ActivateCooldown());

            if (_isInteracting)
            {
                _interactingTarget.InteractOnServer(this);
                return;
            }

            _interactingTarget = null;

            foreach(Interactable interactable in _interactables)
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