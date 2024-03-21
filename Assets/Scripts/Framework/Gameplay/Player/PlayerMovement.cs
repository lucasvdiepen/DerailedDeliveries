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
        [SerializeField]
        private float _speed = 2;

        [SerializeField]
        private float _maxSpeed = 2;

        private PlayerInputParser _playerInputParser;
        private Rigidbody _rigidbody;
        private Vector2 _playerInput;

        private void Awake()
        {
            _rigidbody = gameObject.GetComponent<Rigidbody>();
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
            Vector3 newForce = new Vector3(_playerInput.x, 0, _playerInput.y) * _speed;

            newForce += _rigidbody.velocity;

            newForce.x = Mathf.Clamp(newForce.x, _maxSpeed * -1, _maxSpeed);
            newForce.z = Mathf.Clamp(newForce.z, _maxSpeed * -1, _maxSpeed);

            _rigidbody.velocity = newForce;
        }
    }
}