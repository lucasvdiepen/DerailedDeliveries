using UnityEngine;

namespace DerailedDeliveries.Framework.InputParser
{
    public class PlayerInputTest : MonoBehaviour
    {
        private PlayerInputParser _playerInputParser;

        private void Awake() => _playerInputParser = GetComponent<PlayerInputParser>();

        private void OnEnable()
        {
            _playerInputParser.OnInteract += OnInteract;
            _playerInputParser.OnMove += OnMove;
        }

        private void OnDisable()
        {
            _playerInputParser.OnInteract -= OnInteract;
            _playerInputParser.OnMove -= OnMove;
        }

        private void OnInteract() => Debug.Log("Interact");

        private void OnMove(Vector2 direction) => Debug.Log($"Move: {direction}");
    }
}