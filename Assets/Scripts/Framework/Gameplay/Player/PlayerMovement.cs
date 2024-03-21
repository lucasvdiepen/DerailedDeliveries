using FishNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DerailedDeliveries.Framework.InputParser;
using FishNet.Object;

namespace DerailedDeliveries.Framework.Gameplay.Player
{
    /// <summary>
    /// A class responsible for handling the player's movement.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(NetworkObject))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _playerRigidBody;

        [SerializeField]
        private PlayerInputParser _playerInputParser;

        [SerializeField]
        private Vector2 _playerVelocity;

        [SerializeField]
        private float _playerSpeed = 2;

        private void Awake()
        {
            if(_playerRigidBody == null)
                _playerRigidBody = gameObject.GetComponent<Rigidbody>();

            if(_playerInputParser == null)
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
            InstanceFinder.TimeManager.OnTick -= UpdateVelocity;
        }

        private void SetMovementVector(Vector2 playerInput) => _playerVelocity = playerInput;

        private void UpdateVelocity()
        {
            if (_playerVelocity == Vector2.zero)
                return;

            _playerRigidBody.velocity = new Vector3
            (
                _playerVelocity.x * _playerSpeed,
                _playerRigidBody.velocity.y, 
                _playerVelocity.y * _playerSpeed
            );
        }
    }
}