using FishNet.Object;
using UnityEngine;
using FishNet;

using DerailedDeliveries.Framework.InputParser;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    /// <summary>
    /// A class responsible for handling the player's movement.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInputParser))]
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody _playerRigidBody;

        private PlayerInputParser _playerInputParser;

        private Vector2 _playerInput;

        [SerializeField]
        private float _playerSpeed = 2;

        private void Awake()
        {
            _playerRigidBody = gameObject.GetComponent<Rigidbody>();
            _playerInputParser = gameObject.GetComponent<PlayerInputParser>();
        }

        private void OnEnable()
        {
            _playerInputParser.OnMove += SetMovementVector;
            InstanceFinder.TimeManager.OnTick += UpdateVelocity;
        }

        private void OnDisable()
        {
            _playerInputParser.OnMove -= SetMovementVector;

            if (InstanceFinder.TimeManager != null)
                InstanceFinder.TimeManager.OnTick -= UpdateVelocity;
        }

        private void SetMovementVector(Vector2 playerInput) => _playerInput = playerInput;

        private void UpdateVelocity()
        {
            if (_playerInput == Vector2.zero)
                return;

            _playerRigidBody.velocity = new Vector3
            (
                _playerInput.x * _playerSpeed,
                _playerRigidBody.velocity.y,
                _playerInput.y * _playerSpeed
            );
        }
    }
}