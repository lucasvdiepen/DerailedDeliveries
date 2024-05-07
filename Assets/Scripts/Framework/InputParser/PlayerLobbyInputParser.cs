using FishNet.Object;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

using DerailedDeliveries.Framework.StateMachine;
using DerailedDeliveries.Framework.StateMachine.States;

namespace DerailedDeliveries.Framework.InputParser
{
    /// <summary>
    /// A class responsible for parsing player input during the lobby screens and invoking events based on the input.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerLobbyInputParser : NetworkBehaviour
    {
        /// <summary>
        /// An event invoked when the player pressed the leave button.
        /// </summary>
        public Action OnLeave;

        private PlayerInput _playerInput;
        private bool _isEnabled;

        private void Awake() => _playerInput = GetComponent<PlayerInput>();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();

            if(!IsOwner)
                return;

            StateMachine.StateMachine.Instance.OnStateChanged += UpdateIsEnabled;
            UpdateIsEnabled(StateMachine.StateMachine.Instance.CurrentState);

            _playerInput.actions["Leave"].performed += Leave;
            _playerInput.onDeviceLost += DeviceLost;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnStopClient()
        {
            base.OnStopClient();

            if(!IsOwner)
                return;

            if(StateMachine.StateMachine.Instance != null)
                StateMachine.StateMachine.Instance.OnStateChanged -= UpdateIsEnabled;

            _playerInput.actions["Leave"].performed -= Leave;
            _playerInput.onDeviceLost -= DeviceLost;
        }

        private void UpdateIsEnabled(State state) => _isEnabled = state is HostState || state is JoinState;

        private void Leave(InputAction.CallbackContext context) => Leave();

        private void Leave()
        {
            if(!_isEnabled)
                return;

            OnLeave?.Invoke();
        }

        private void DeviceLost(PlayerInput playerInput) => Leave();
    }
}