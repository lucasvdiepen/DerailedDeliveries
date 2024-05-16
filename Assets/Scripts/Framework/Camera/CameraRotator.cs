using UnityEngine;

using DerailedDeliveries.Framework.StateMachine;
using DerailedDeliveries.Framework.StateMachine.States;

namespace DerailedDeliveries.Framework.Camera
{
    /// <summary>
    /// A class responsible for rotating the camera for the background of the menu screens.
    /// </summary>
    public class CameraRotator : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 5f;

        private void OnEnable() => StateMachine.StateMachine.Instance.OnStateChanged += OnStateChanged;

        private void OnDisable()
        {
            if(StateMachine.StateMachine.Instance == null)
                return;

            StateMachine.StateMachine.Instance.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(State state) => gameObject.SetActive(state is not GameState);

        private void Update() => transform.Rotate(Vector3.up, _speed * Time.deltaTime);
    }
}