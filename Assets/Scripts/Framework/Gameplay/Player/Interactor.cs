using FishNet.Object.Synchronizing;
using FishNet.Connection;
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
    [RequireComponent(typeof(PlayerInputParser), typeof(SphereCollider))]
    public class Interactor : NetworkBehaviour
    {
        /// <summary>
        /// Returns the GrabbingAnchor <see cref="Transform"/> of this <see cref="Interactor"/>.
        /// </summary>
        public NetworkBehaviour GrabbingAnchor => _grabbingAnchor;

        /// <summary>
        /// A getter that returns the <see cref="Interactor"/>'s InteractingTarget.
        /// </summary>
        public Interactable InteractingTarget => _interactingTarget;

        [SerializeField]
        private Interactable _interactingTarget;

        [SerializeField]
        private NetworkBehaviour _grabbingAnchor;

        [SerializeField]
        private float _cooldown = .2f;

        [SerializeField]
        private SphereCollider _collider;

        private PlayerInputParser _inputParser;
        private bool _isInteracting;
        private bool _isOnCooldown;

        private void Awake()
        {
            _inputParser = GetComponent<PlayerInputParser>();

            if (_collider == null)
                _collider = GetComponent<SphereCollider>();
        }

        private void OnEnable() => _inputParser.OnInteract += UseInteractable;

        private void OnDisable() => _inputParser.OnInteract -= UseInteractable;

        private void UseInteractable()
        {
            Collider[] interactables = Physics.OverlapSphere(_collider.transform.position, _collider.radius);

            if (_isOnCooldown || !_isInteracting && interactables.Length == 0)
                return;

            StartCoroutine(ActivateCooldown());

            if (_isInteracting && _interactingTarget != null)
            {
                _interactingTarget.InteractOnServer(this);
                return;
            }

            foreach(Collider colliding in interactables)
            {
                if (!colliding.TryGetComponent(out Interactable interactable))
                    continue;

                if (!interactable.CheckIfInteractable(this))
                    continue;

                interactable.InteractOnServer(this);
                break;
            }
        }

        /// <summary>
        /// A function that sends an RPC to the owning client of this <see cref="Interactor"/> that needs this 
        /// information.
        /// </summary>
        /// <param name="connection">The connection to target the RPC to.</param>
        /// <param name="interactable">The new <see cref="Interactable"/> Target.</param>
        /// <param name="isInteracting">The new Interacting bool status.</param>
        [TargetRpc(RunLocally = true)]
        public void UpdateInteractingTarget(NetworkConnection connection, Interactable interactable, bool isInteracting)
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