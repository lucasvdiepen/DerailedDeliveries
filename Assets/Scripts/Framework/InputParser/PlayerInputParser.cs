using FishNet.Object;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

using DerailedDeliveries.Framework.StateMachine;
using DerailedDeliveries.Framework.StateMachine.States;

namespace DerailedDeliveries.Framework.InputParser
{
    /// <summary>
    /// A class responsible for parsing player input and invoking events based on the input.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputParser : NetworkBehaviour
    {
        /// <summary>
        /// An event invoked when the player pressed the interact button.
        /// </summary>
        public Action OnInteract;

        /// <summary>
        /// An event invoked when the player pressed the use button.
        /// </summary>
        public Action OnUse;

        /// <summary>
        /// An event invoked when the player gives input to move.
        /// Note that this event is only called when the input changes and not every frame.
        /// </summary>
        public Action<Vector2> OnMove;

        /// <summary>
        /// The direction the player is currently moving in.
        /// </summary>
        public Vector2 MoveDirection { get; private set; }

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

            _playerInput.actions["Move"].performed += Move;
            _playerInput.actions["Interact"].performed += Interact;
            _playerInput.actions["Use"].performed += Use;
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

            _playerInput.actions["Move"].performed -= Move;
            _playerInput.actions["Interact"].performed -= Interact;
            _playerInput.actions["Use"].performed -= Use;
        }

        private void UpdateIsEnabled(State state) => _isEnabled = state is GameState;

        private void Move(InputAction.CallbackContext context)
        {
            if(!_isEnabled)
                return;

            MoveDirection = context.ReadValue<Vector2>();

            OnMove?.Invoke(MoveDirection);
        }

        private void Interact(InputAction.CallbackContext context)
        {
            if(!_isEnabled)
                return;

            OnInteract?.Invoke();
        }

        private void Use(InputAction.CallbackContext context)
        {
            if(!_isEnabled)
                return;

            OnUse?.Invoke();
        }
    }
}